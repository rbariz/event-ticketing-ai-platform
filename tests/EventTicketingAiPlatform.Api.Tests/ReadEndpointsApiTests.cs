using EventTicketingAiPlatform.Contracts.Scans;
using EventTicketingAiPlatform.Contracts.ScanValidation;
using EventTicketingAiPlatform.Contracts.Tickets;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace EventTicketingAiPlatform.Api.Tests
{
    public sealed class ReadEndpointsApiTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ReadEndpointsApiTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetScans_Should_Return_Ok()
        {
            var response = await _client.GetAsync("/api/scans");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<List<ScanAttemptResponse>>();
            body.Should().NotBeNull();
        }

        [Fact]
        public async Task GetTicketByCode_Should_Return_Ok_For_Seeded_Ticket()
        {
            var response = await _client.GetAsync("/api/tickets/by-code/TCK-VALID-001");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<TicketDetailsResponse>();
            body.Should().NotBeNull();
            body!.TicketCode.Should().Be("TCK-VALID-001");
        }

        [Fact]
        public async Task GetTicketByCode_Should_Return_404_For_Unknown_Ticket()
        {
            var response = await _client.GetAsync("/api/tickets/by-code/UNKNOWN");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        }

        [Fact]
        public async Task GetDashboardSummary_Should_Return_Ok()
        {
            var response = await _client.GetAsync("/api/dashboard/summary");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetRecentScans_Should_Return_Ok()
        {
            var response = await _client.GetAsync("/api/scans/recent?count=5");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetScans_Should_Support_Decision_Filter()
        {
            var response = await _client.GetAsync("/api/scans?decision=Rejected");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task GetDashboardSummary_Should_Support_Gate_Filter()
        {
            var response = await _client.GetAsync("/api/dashboard/summary?gateId=GATE-A");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task AnalyzeScanWithAgent_Should_Return_Ok_For_Existing_Scan()
        {
            var validateResponse = await _client.PostAsJsonAsync(
    "/api/scans/validate",
    new ValidateTicketScanRequest(
        "UNKNOWN-AGENT-001",
        "DEV-AGENT",
        "GATE-X",
        DateTime.UtcNow,
        "api-test"));

            var scanResult = await validateResponse.Content
                .ReadFromJsonAsync<ValidateTicketScanResponse>();

            var response = await _client.PostAsync(
                $"/api/agent/analyze-scan/{scanResult!.ScanAttemptId}?lang=en",
                null);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
