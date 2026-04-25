using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.Risk
{
    public sealed class AntifraudRiskAssessment
    {
        public int RiskScore { get; init; }
        public string RiskLevel { get; init; } = "";
        public string RecommendedAction { get; init; } = "";
        public List<string> RiskSignals { get; init; } = [];
    }
}
