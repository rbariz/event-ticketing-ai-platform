using EventTicketingAiPlatform.Application.Agent;
using EventTicketingAiPlatform.Application.Options;
using EventTicketingAiPlatform.Contracts.Risk;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace EventTicketingAiPlatform.Infrastructure.AI
{
    public sealed class OpenAiAgentEnrichmentService : IAgentEnrichmentService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAiOptions _options;
        private readonly ILogger<OpenAiAgentEnrichmentService> _logger;
        private readonly RuleBasedAgentEnrichmentService _fallback = new();

        public OpenAiAgentEnrichmentService(
            HttpClient httpClient,
            IOptions<OpenAiOptions> options,
            ILogger<OpenAiAgentEnrichmentService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<AgentEnrichment> EnrichAsync(
            RiskAssessmentResponse risk,
            AgentDecision decision,
            string language,
            CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.ApiKey))
                return await _fallback.EnrichAsync(risk, decision, language, cancellationToken);

            var outputLanguage = language.Equals("fr", StringComparison.OrdinalIgnoreCase)
                ? "French"
                : "English";

            var prompt = $$"""
You are an antifraud operations assistant.

Your role is to enrich an existing antifraud agent decision for a human operator.
You MUST NOT change the decision, severity, or actions.

Return valid JSON only.

JSON format:
{
  "operatorSummary": "...",
  "suggestedNextActions": ["...", "..."],
  "confidenceScore": 0.85,
  "businessImpact": "..."
}

Rules:
- Use only provided data.
- Do not invent facts.
- Do not change severity.
- Do not change actions.
- Keep suggestedNextActions practical and short.
- confidenceScore must be between 0.50 and 0.95.
- Write operatorSummary, suggestedNextActions and businessImpact in {{outputLanguage}}.

Risk:
RiskScore: {{risk.RiskScore}}
RiskLevel: {{risk.RiskLevel}}
RecommendedAction: {{risk.RecommendedAction}}
RiskSignals: {{string.Join(", ", risk.RiskSignals)}}
RiskExplanation: {{risk.RiskExplanation}}

Agent decision:
Severity: {{decision.Severity}}
Actions: {{string.Join(", ", decision.Actions)}}
Reason: {{decision.Reason}}
RequiresHumanReview: {{decision.RequiresHumanReview}}
""";

            var request = new
            {
                model = _options.Model,
                input = prompt,
                max_output_tokens = 250
            };

            var stopwatch = Stopwatch.StartNew();

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/responses");

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.ApiKey);

            httpRequest.Content = JsonContent.Create(request);

            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogInformation(
                "OpenAI agent enrichment completed. StatusCode={StatusCode}, ElapsedMs={ElapsedMs}, Model={Model}",
                (int)response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                _options.Model);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "OpenAI agent enrichment failed. StatusCode={StatusCode}, ErrorBody={ErrorBody}",
                    (int)response.StatusCode,
                    responseJson);

                return await _fallback.EnrichAsync(risk, decision, language, cancellationToken);
            }

            try
            {
                using var doc = JsonDocument.Parse(responseJson);

                var text = doc.RootElement
                    .GetProperty("output")[0]
                    .GetProperty("content")[0]
                    .GetProperty("text")
                    .GetString();

                if (string.IsNullOrWhiteSpace(text))
                    return await _fallback.EnrichAsync(risk, decision, language, cancellationToken);

                var dto = JsonSerializer.Deserialize<OpenAiAgentEnrichmentDto>(
                    text,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (dto is null)
                    return await _fallback.EnrichAsync(risk, decision, language, cancellationToken);

                return new AgentEnrichment(
                    dto.OperatorSummary ?? "",
                    dto.SuggestedNextActions ?? [],
                    Clamp(dto.ConfidenceScore),
                    dto.BusinessImpact ?? "",
                    "OpenAI");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "OpenAI agent enrichment parsing failed.");
                return await _fallback.EnrichAsync(risk, decision, language, cancellationToken);
            }
        }

        private static decimal Clamp(decimal value)
        {
            if (value < 0.50m)
                return 0.50m;

            if (value > 0.95m)
                return 0.95m;

            return value;
        }

        private sealed class OpenAiAgentEnrichmentDto
        {
            public string? OperatorSummary { get; set; }
            public List<string>? SuggestedNextActions { get; set; }
            public decimal ConfidenceScore { get; set; }
            public string? BusinessImpact { get; set; }
        }
    }
}
