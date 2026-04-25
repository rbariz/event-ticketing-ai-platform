using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.Domain.Entities
{
    public sealed class Event
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartsAtUtc { get; set; }
        public DateTime EndsAtUtc { get; set; }
    }
}
