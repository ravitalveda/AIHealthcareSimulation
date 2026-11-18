namespace AIHealthcareSimulation.Contracts;

public record HospitalSimulationBehavior
{
    public Guid SimulationId { get; init; }

    public string ScenarioName { get; init; } = string.Empty;

    public string StaffRole { get; init; } = string.Empty;

    public string ObservedBehavior { get; init; } = string.Empty;

    public string ExpectedBehavior { get; init; } = string.Empty;

    public bool MetExpectedBehavior { get; init; }

    public DateTime ObservedAtUtc { get; init; }
}
