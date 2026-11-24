using AIHealthcareSimulation.Api.DTOs;
using AIHealthcareSimulation.Contracts;

namespace AIHealthcareSimulation.Api.Endpoints;

/// <summary>Maps endpoints for browsing training scenarios.</summary>
public static class ScenarioEndpoints
{
    public static IEndpointRouteBuilder MapScenarioEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
                "/api/scenarios",
                () => TypedResults.Ok(
                    HospitalSimulationScenarioCatalog.All
                        .Select(scenario => new ScenarioResponse(
                            scenario.Id,
                            scenario.Name,
                            scenario.StaffRole,
                            scenario.ExpectedBehavior,
                            scenario.BehaviorOptions
                                .Select(option => new BehaviorOptionResponse(
                                    option.Id,
                                    option.Description))
                                .ToArray()))
                        .ToArray()))
            .WithName("GetTrainingScenarios")
            .WithSummary("List available hospital simulation training scenarios")
            .WithDescription("Returns the in-memory sample scenarios available in Phase 1.")
            .Produces<ScenarioResponse[]>(StatusCodes.Status200OK);

        return endpoints;
    }
}
