using EventTicketingAiPlatform.Contracts.Scans;
using EventTicketingAiPlatform.Contracts.Tickets;
using EventTicketingAiPlatform.Mobile.Scanner.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace EventTicketingAiPlatform.Mobile.Scanner.Services
{
    public sealed class ApiSettings
    {
        public string BaseUrl { get; set; } = "";
    }

}
