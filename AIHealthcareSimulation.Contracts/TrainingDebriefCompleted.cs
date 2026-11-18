namespace AIHealthcareSimulation.Contracts;

public record TrainingDebriefCompleted
{
    public Guid SimulationId { get; init; }

    public string ScenarioName { get; init; } = string.Empty;

    public string StaffRole { get; init; } = string.Empty;

    public string ObservedBehavior { get; init; } = string.Empty;

    public string ExpectedBehavior { get; init; } = string.Empty;

    public TrainingOutcome Outcome { get; init; }

    public string AiReflectionQuestion { get; init; } = string.Empty;

    public DateTime CompletedAtUtc { get; init; }
}
