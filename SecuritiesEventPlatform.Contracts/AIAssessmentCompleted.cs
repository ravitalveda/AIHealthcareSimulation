
namespace SecuritiesEventPlatform.Contracts;

public record AIAssessmentCompleted
{
    public Guid IncidentId { get; init; }

    public string IncidentType { get; init; } = string.Empty;

    public string RiskLevel { get; init; } = string.Empty;

    public string AiAssessment { get; init; } = string.Empty;

    public DateTime CompletedAtUtc { get; init; }
}
