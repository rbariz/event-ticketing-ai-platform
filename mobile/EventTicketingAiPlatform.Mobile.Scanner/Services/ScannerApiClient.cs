using EventTicketingAiPlatform.Contracts.Scans;
using EventTicketingAiPlatform.Contracts.Tickets;
using EventTicketingAiPlatform.Mobile.Scanner.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace EventTicketingAiPlatform.Mobile.Scanner.Services
{
    public sealed class ScannerApiClient
    {
        private readonly HttpClient _http;
        private readonly AppSettings _settings;

        public ScannerApiClient(
            HttpClient http,
            IOptions<AppSettings> options)
        {
            _http = http;
            _settings = options.Value;

            _http.BaseAddress = new Uri(_settings.Api.BaseUrl);
        }

        // Validate scan
        public async Task<ValidateScanResponse?> ValidateAsync(
            ValidateScanRequest request)
        {
            var response = await _http.PostAsJsonAsync(
                "/api/scans/validate",
                request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ValidateScanResponse>();
        }

        //  Get risk details
        public async Task<RiskResponse?> GetRiskAsync(
            Guid scanId,
            string lang = "en")
        {
            return await _http.GetFromJsonAsync<RiskResponse>(
                $"/api/scans/{scanId}/risk?lang={lang}");
        }


        public async Task<List<ScanAttemptResponse>> GetRecentAsync(
    int count = 20,
    CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<List<ScanAttemptResponse>>(
                $"/api/scans/recent?count={count}",
                cancellationToken) ?? [];
        }

        public async Task<TicketDetailsResponse?> GetTicketAsync(
            string ticketCode,
            CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<TicketDetailsResponse>(
                $"/api/tickets/by-code/{Uri.EscapeDataString(ticketCode.Trim())}",
                cancellationToken);
        }

        public async Task<List<ScanAttemptResponse>> GetScansAsync(
            CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<List<ScanAttemptResponse>>(
                "/api/scans",
                cancellationToken) ?? [];
        }

        public async Task<AgentDecisionResponse?> AnalyzeWithAgentAsync(
    Guid scanId,
    string lang,
    CancellationToken cancellationToken = default)
        {
            var response = await _http.PostAsync(
                $"/api/agent/analyze-scan/{scanId}?lang={Uri.EscapeDataString(lang)}",
                content: null,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<AgentDecisionResponse>(
                cancellationToken: cancellationToken);
        }
        public async Task<List<AgentNotificationResponse>> GetAgentNotificationsAsync(
    bool unreadOnly = true,
    int count = 20,
    CancellationToken cancellationToken = default)
        {
            return await _http.GetFromJsonAsync<List<AgentNotificationResponse>>(
                $"/api/agent/notifications?unreadOnly={unreadOnly.ToString().ToLowerInvariant()}&count={count}",
                cancellationToken) ?? [];
        }

        public async Task MarkAgentNotificationAsReadAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var response = await _http.PostAsync(
                $"/api/agent/notifications/{id}/mark-read",
                content: null,
                cancellationToken);

            response.EnsureSuccessStatusCode();
        }
    }

}
