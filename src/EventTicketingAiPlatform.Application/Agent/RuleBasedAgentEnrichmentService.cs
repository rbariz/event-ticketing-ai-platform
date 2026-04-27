using EventTicketingAiPlatform.Contracts.Risk;

namespace EventTicketingAiPlatform.Application.Agent
{
    public sealed class RuleBasedAgentEnrichmentService : IAgentEnrichmentService
    {
        public Task<AgentEnrichment> EnrichAsync(
            RiskAssessmentResponse risk,
            AgentDecision decision,
            string language,
            CancellationToken cancellationToken = default)
        {
            var isFr = language.Equals("fr", StringComparison.OrdinalIgnoreCase);

            var summary = BuildSummary(risk, decision, isFr);
            var actions = BuildSuggestedActions(risk, decision, isFr);
            var impact = BuildBusinessImpact(risk, decision, isFr);
            var confidence = BuildConfidenceScore(risk, decision);

            return Task.FromResult(new AgentEnrichment(
                summary,
                actions,
                confidence,
                impact,
                "RuleBased"));
        }

        private static string BuildSummary(
            RiskAssessmentResponse risk,
            AgentDecision decision,
            bool isFr)
        {
            if (risk.RiskSignals.Count == 0)
            {
                return isFr
                    ? "Aucun signal antifraude significatif n’a été détecté."
                    : "No significant antifraud signal was detected.";
            }

            return isFr
                ? $"Le scan présente un risque {risk.RiskLevel.ToLower()} avec les signaux suivants : {string.Join(", ", risk.RiskSignals)}."
                : $"The scan shows a {risk.RiskLevel.ToLower()} risk with the following signals: {string.Join(", ", risk.RiskSignals)}.";
        }

        private static IReadOnlyList<string> BuildSuggestedActions(
            RiskAssessmentResponse risk,
            AgentDecision decision,
            bool isFr)
        {
            var signals = risk.RiskSignals
                .Select(x => x.ToLowerInvariant())
                .ToHashSet();

            if (signals.Contains("duplicate_scan") || signals.Contains("multi_device"))
            {
                return isFr
                    ? [
                        "Vérifier l’identité du porteur du billet.",
                    "Contrôler les tentatives de scan liées au même billet.",
                    "Maintenir l’incident ouvert jusqu’à validation opérateur."
                      ]
                    : [
                        "Verify the ticket holder identity.",
                    "Check related scan attempts for the same ticket.",
                    "Keep the incident open until operator review is completed."
                      ];
            }

            if (signals.Contains("unknown_ticket"))
            {
                return isFr
                    ? [
                        "Refuser l’accès au porteur.",
                    "Vérifier si le code provient d’un canal officiel.",
                    "Signaler le cas à l’équipe opérations."
                      ]
                    : [
                        "Deny access to the holder.",
                    "Verify whether the code comes from an official channel.",
                    "Escalate the case to the operations team."
                      ];
            }

            if (signals.Contains("expired_ticket") || signals.Contains("already_used"))
            {
                return isFr
                    ? [
                        "Confirmer le statut du billet.",
                    "Informer l’opérateur de porte.",
                    "Clôturer l’incident si aucune fraude n’est confirmée."
                      ]
                    : [
                        "Confirm the ticket status.",
                    "Inform the gate operator.",
                    "Resolve the incident if no fraud is confirmed."
                      ];
            }

            if (decision.RequiresHumanReview)
            {
                return isFr
                    ? [
                        "Effectuer une revue manuelle.",
                    "Comparer le scan avec l’historique du billet.",
                    "Documenter la décision finale."
                      ]
                    : [
                        "Perform manual review.",
                    "Compare the scan with ticket history.",
                    "Document the final decision."
                      ];
            }

            return isFr
                ? ["Aucune action immédiate requise."]
                : ["No immediate action required."];
        }

        private static string BuildBusinessImpact(
            RiskAssessmentResponse risk,
            AgentDecision decision,
            bool isFr)
        {
            if (decision.RequiresHumanReview)
            {
                return isFr
                    ? "Risque potentiel d’accès non autorisé ou de perte de contrôle opérationnel si le cas n’est pas traité."
                    : "Potential risk of unauthorized access or operational control loss if the case is not handled.";
            }

            return isFr
                ? "Impact opérationnel faible."
                : "Low operational impact.";
        }

        private static decimal BuildConfidenceScore(
            RiskAssessmentResponse risk,
            AgentDecision decision)
        {
            var baseScore = risk.RiskScore / 100m;

            if (decision.RequiresHumanReview)
                baseScore += 0.10m;

            if (risk.RiskSignals.Count >= 2)
                baseScore += 0.10m;

            return Math.Min(0.95m, Math.Max(0.50m, baseScore));
        }
    }
}
