using System.ComponentModel.DataAnnotations;

namespace AIHealthcareSimulation.Api.DTOs;

/// <summary>Starts a training simulation using a scenario and observed behavior.</summary>
public sealed record CreateSimulationRequest
{
    /// <summary>Client-generated identifier used to correlate the asynchronous result.</summary>
    [Required]
    public Guid SimulationId { get; init; }

    /// <summary>Identifier of the selected scenario.</summary>
    [Required, StringLength(64)]
    public string ScenarioId { get; init; } = string.Empty;

    /// <summary>Identifier of the simulated behavior observed in the scenario.</summary>
    [Required, StringLength(64)]
    public string BehaviorOptionId { get; init; } = string.Empty;
}

/// <summary>Confirms that a simulation was accepted for asynchronous processing.</summary>
public sealed record SimulationAcceptedResponse(Guid SimulationId, string Status);
