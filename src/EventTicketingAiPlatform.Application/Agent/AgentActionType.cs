using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.Agent
{
    public enum AgentActionType
    {
        NoAction = 0,
        Monitor = 1,
        RequireManualReview = 2,
        NotifyOps = 3,
        CreateIncident = 4,
        BlockAndEscalate = 5
    }
}
