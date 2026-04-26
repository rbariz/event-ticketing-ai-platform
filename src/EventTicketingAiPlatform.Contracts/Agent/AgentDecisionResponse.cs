using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Contracts.Agent
{
    public sealed record AgentDecisionResponse(
    string Severity,
    IReadOnlyList<string> Actions,
    string Reason,
    bool RequiresHumanReview);
}
