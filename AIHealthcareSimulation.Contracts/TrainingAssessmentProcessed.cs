namespace AIHealthcareSimulation.Contracts;

public enum TrainingOutcome
{
    MeetsExpectations,
    PracticeRecommended
}

public record TrainingAssessmentProcessed
{
    public Guid SimulationId { get; init; }

    public string ScenarioName { get; init; } = string.Empty;

    public string StaffRole { get; init; } = string.Empty;

    public string ObservedBehavior { get; init; } = string.Empty;

    public string ExpectedBehavior { get; init; } = string.Empty;

    public TrainingOutcome Outcome { get; init; }

    public DateTime AssessedAtUtc { get; init; }
}
