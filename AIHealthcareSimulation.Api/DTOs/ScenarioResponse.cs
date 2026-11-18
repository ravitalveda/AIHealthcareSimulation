namespace AIHealthcareSimulation.Api.DTOs;

/// <summary>Describes a training scenario and its selectable simulated behaviors.</summary>
public sealed record ScenarioResponse(
    string Id,
    string Name,
    string StaffRole,
    string ExpectedBehavior,
    IReadOnlyList<BehaviorOptionResponse> BehaviorOptions);

/// <summary>Describes one behavior that can be selected in a simulated scenario.</summary>
public sealed record BehaviorOptionResponse(string Id, string Description);
