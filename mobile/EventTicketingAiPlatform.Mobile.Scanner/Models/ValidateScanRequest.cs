using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Mobile.Scanner.Models
{
    public sealed class ValidateScanRequest
    {
        public string TicketCode { get; set; } = "";
        public string DeviceId { get; set; } = "MOBILE-01";
        public string GateId { get; set; } = "MOBILE";
        public DateTime ScannedAtUtc { get; set; } = DateTime.UtcNow;
        public string Source { get; set; } = "mobile";
    }
}
