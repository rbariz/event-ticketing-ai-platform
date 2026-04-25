using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.Options
{
    public sealed class OpenAiOptions
    {
        public bool Enabled { get; init; }
        public string ApiKey { get; init; } = string.Empty;
        public string Model { get; init; } = "gpt-4.1-mini";
    }
}
