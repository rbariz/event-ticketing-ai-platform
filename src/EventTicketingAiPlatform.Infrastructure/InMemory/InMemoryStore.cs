using EventTicketingAiPlatform.Application.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Infrastructure.InMemory
{
    public sealed class InMemoryStore
    {
        public List<Ticket> Tickets { get; } = [];
        public List<ScanAttempt> ScanAttempts { get; } = [];

        public List<AgentDecisionLog> AgentDecisionLogs { get; } = [];

        public List<AgentNotification> AgentNotifications { get; } = [];
    }
}
