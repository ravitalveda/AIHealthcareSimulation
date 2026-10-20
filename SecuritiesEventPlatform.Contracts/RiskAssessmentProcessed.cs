namespace SecuritiesEventPlatform.Contracts
{
    public record RiskAssessmentProcessed
    {
        public Guid IncidentId { get; init; }

        public string IncidentType { get; init; } = string.Empty;

        public string RiskLevel { get; init; } = string.Empty;

        public DateTime AssessedAtUtc { get; init; }
    }
}
