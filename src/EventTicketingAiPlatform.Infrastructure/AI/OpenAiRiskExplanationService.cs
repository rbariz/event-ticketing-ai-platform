using EventTicketingAiPlatform.Application.Options;
using EventTicketingAiPlatform.Application.Risk;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Infrastructure.AI
{
   public sealed class OpenAiRiskExplanationService : IRiskExplanationService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAiOptions _options;
        private readonly ILogger<OpenAiRiskExplanationService> _logger;

        public OpenAiRiskExplanationService(
            HttpClient httpClient,
            IOptions<OpenAiOptions> options,
            ILogger<OpenAiRiskExplanationService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<RiskExplanationResult> GenerateExplanationAsync(
            AntifraudRiskAssessment risk,
            string language,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                _logger.LogInformation(
                    "OpenAI risk explanation skipped. Enabled={Enabled}, HasApiKey={HasApiKey}",
                    _options.Enabled,
                    !string.IsNullOrWhiteSpace(_options.ApiKey));

                return BuildFallback(risk, language);
            }

            var outputLanguage = language.Equals("fr", StringComparison.OrdinalIgnoreCase)
                ? "French"
                : "English";

            var prompt = $$"""
                    You are an antifraud operations assistant.

                    Analyze the risk assessment and return a structured JSON.

                    Rules:
                    - Do NOT invent facts.
                    - Use only provided risk data.
                    - Do NOT change RecommendedAction.
                    - Keep responses short.
                    - Output MUST be valid JSON only.
                    - Write summary and operatorMessage in {{outputLanguage}}.
                    - Keep confidence in English: Low, Medium, High.

                    JSON format:
                    {
                      "summary": "...",
                      "operatorMessage": "...",
                      "confidence": "Low | Medium | High"
                    }

                    Risk data:
                    RiskScore: {{risk.RiskScore}}
                    RiskLevel: {{risk.RiskLevel}}
                    RecommendedAction: {{risk.RecommendedAction}}
                    RiskSignals: {{string.Join(", ", risk.RiskSignals)}}
                    """;

            var request = new
            {
                model = _options.Model,
                input = prompt,
                max_output_tokens = 180
            };

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/responses");

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.ApiKey);

            httpRequest.Content = JsonContent.Create(request);

            using var response = await _httpClient.SendAsync(
                httpRequest,
                cancellationToken);

            _logger.LogInformation(
                "OpenAI risk explanation request completed. StatusCode={StatusCode}, ElapsedMs={ElapsedMs}, Model={Model}",
                (int)response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                _options.Model);

            if (!response.IsSuccessStatusCode)
            {
                //_logger.LogWarning(
                //    "OpenAI risk explanation failed. StatusCode={StatusCode}, ElapsedMs={ElapsedMs}",
                //    (int)response.StatusCode,
                //    stopwatch.ElapsedMilliseconds);

                //return BuildFallback(risk, language);
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogWarning(
                    "OpenAI risk explanation failed. StatusCode={StatusCode}, ElapsedMs={ElapsedMs}, Model={Model}, ErrorBody={ErrorBody}",
                    (int)response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    _options.Model,
                    errorBody);

                return BuildFallback(risk, language);
            }

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            try
            {
                using var doc = JsonDocument.Parse(responseJson);

                var text = doc.RootElement
                    .GetProperty("output")[0]
                    .GetProperty("content")[0]
                    .GetProperty("text")
                    .GetString();

                if (string.IsNullOrWhiteSpace(text))
                {
                    _logger.LogWarning(
                        "OpenAI risk explanation returned empty text. ElapsedMs={ElapsedMs}",
                        stopwatch.ElapsedMilliseconds);

                    return BuildFallback(risk, language);
                }

                var result = JsonSerializer.Deserialize<RiskExplanationResult>(
                    text,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (result is null)
                {
                    _logger.LogWarning(
                        "OpenAI risk explanation JSON deserialized to null. ElapsedMs={ElapsedMs}",
                        stopwatch.ElapsedMilliseconds);

                    return BuildFallback(risk, language);
                }

                _logger.LogInformation(
                    "OpenAI risk explanation generated. RiskLevel={RiskLevel}, RiskScore={RiskScore}, Confidence={Confidence}, ElapsedMs={ElapsedMs}",
                    risk.RiskLevel,
                    risk.RiskScore,
                    result.Confidence,
                    stopwatch.ElapsedMilliseconds);

                return result with { Provider = "OpenAI" };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "OpenAI risk explanation parsing failed. ElapsedMs={ElapsedMs}",
                    stopwatch.ElapsedMilliseconds);

                return BuildFallback(risk, language);
            }
        }

        private static RiskExplanationResult BuildFallback(
            AntifraudRiskAssessment risk,
            string language)
        {
            var isFr = language.Equals("fr", StringComparison.OrdinalIgnoreCase);

            if (risk.RiskSignals.Count == 0)
            {
                return new RiskExplanationResult(
                    isFr ? "Aucune activité suspecte" : "No suspicious activity",
                    isFr
                        ? "Aucune activité suspecte détectée. Aucune action immédiate requise."
                        : "No suspicious activity was detected. No immediate action is required.",
                    "High",
                    "Fallback");
            }

            return new RiskExplanationResult(
                isFr
                    ? $"Risque {risk.RiskLevel.ToLower()} détecté"
                    : $"{risk.RiskLevel} risk detected",
                isFr
                    ? $"Risque {risk.RiskLevel.ToLower()} détecté. Action recommandée : {risk.RecommendedAction}. Signaux : {string.Join(", ", risk.RiskSignals)}."
                    : $"{risk.RiskLevel} risk detected. Recommended action: {risk.RecommendedAction}. Main signals: {string.Join(", ", risk.RiskSignals)}.",
                risk.RiskSignals.Count >= 2 ? "High" : "Medium",
                "Fallback");
        }
    }
}
