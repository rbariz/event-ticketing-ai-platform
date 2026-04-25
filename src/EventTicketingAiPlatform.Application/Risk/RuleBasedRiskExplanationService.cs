namespace EventTicketingAiPlatform.Application.Risk
{
    public sealed class RuleBasedRiskExplanationService : IRiskExplanationService
    {
        public Task<RiskExplanationResult> GenerateExplanationAsync(
            AntifraudRiskAssessment risk,
            string language,
            CancellationToken cancellationToken = default)
        {
            var isFr = language.Equals("fr", StringComparison.OrdinalIgnoreCase);

            var summary = risk.RiskSignals.Count == 0
                ? isFr ? "Aucune activité suspecte" : "No suspicious activity"
                : isFr ? $"Risque {risk.RiskLevel.ToLower()} détecté" : $"{risk.RiskLevel} risk detected";

            var message = risk.RiskSignals.Count == 0
                ? isFr
                    ? "Aucune activité suspecte détectée. Aucune action immédiate requise."
                    : "No suspicious activity was detected. No immediate action is required."
                : isFr
                    ? $"Risque {risk.RiskLevel.ToLower()} détecté. Action recommandée : {risk.RecommendedAction}. Signaux : {string.Join(", ", risk.RiskSignals)}."
                    : $"{risk.RiskLevel} risk detected. Recommended action: {risk.RecommendedAction}. Main signals: {string.Join(", ", risk.RiskSignals)}.";

            var confidence = risk.RiskSignals.Count >= 2 ? "High" : "Medium";

            return Task.FromResult(new RiskExplanationResult(
                summary,
                message,
                confidence,
                "RuleBased"));
        }
    }
}
