namespace EventTicketingAiPlatform.Application.Risk
{
    public interface IRiskExplanationService
    {
        Task<RiskExplanationResult> GenerateExplanationAsync(
            AntifraudRiskAssessment risk,
            string language,
            CancellationToken cancellationToken = default);
    }
}
