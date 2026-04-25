using EventTicketingAiPlatform.Contracts.ScanValidation;
using EventTicketingAiPlatform.Infrastructure.InMemory;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace EventTicketingAiPlatform.Api.Tests
{
    public sealed class ScanValidationApiTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ScanValidationApiTests(TestWebApplicationFactory factory)
        {
            // _client = factory.CreateClient();
            using var scope = factory.Services.CreateScope();
            var store = scope.ServiceProvider.GetRequiredService<InMemoryStore>();

            store.Tickets.Clear();
            store.ScanAttempts.Clear();
            InMemorySeed.Seed(store);

            _client = factory.CreateClient();
        }

        [Fact]
        public async Task ValidateScan_Should_Accept_Valid_Ticket()
        {
            var request = new ValidateTicketScanRequest(
                "TCK-VALID-001",
                "DEV-001",
                "GATE-A",
                DateTime.UtcNow,
                "api-test");

            var response = await _client.PostAsJsonAsync("/api/scans/validate", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<ValidateTicketScanResponse>();
            body.Should().NotBeNull();
            body!.Accepted.Should().BeTrue();
            body.ReasonCode.Should().Be("Ok");
        }

        [Fact]
        public async Task ValidateScan_Should_Reject_Unknown_Ticket()
        {
            var request = new ValidateTicketScanRequest(
                "UNKNOWN",
                "DEV-001",
                "GATE-A",
                DateTime.UtcNow,
                "api-test");

            var response = await _client.PostAsJsonAsync("/api/scans/validate", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<ValidateTicketScanResponse>();
            body.Should().NotBeNull();
            body!.Accepted.Should().BeFalse();
            body.ReasonCode.Should().Be("TicketNotFound");
        }

        [Fact]
        public async Task ValidateScan_Should_Return_ProblemDetails_For_Invalid_Request()
        {
            var request = new ValidateTicketScanRequest(
                "",
                "",
                "",
                default,
                "api-test");

            var response = await _client.PostAsJsonAsync("/api/scans/validate", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        }
    }
}
