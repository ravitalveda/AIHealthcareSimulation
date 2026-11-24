using AIHealthcareSimulation.Api.DTOs;
using AIHealthcareSimulation.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AIHealthcareSimulation.Api.Endpoints;

/// <summary>Maps endpoints for starting training simulations.</summary>
public static class SimulationEndpoints
{
    public static IEndpointRouteBuilder MapSimulationEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
                "/api/simulations",
                async Task<Results<
                    JsonHttpResult<SimulationAcceptedResponse>,
                    ValidationProblem>> (
                    CreateSimulationRequest request,
                    IPublishEndpoint publisher,
                    CancellationToken cancellationToken) =>
                {
                    if (request.SimulationId == Guid.Empty)
                    {
                        return TypedResults.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                [nameof(request.SimulationId)] =
                                ["A non-empty simulation ID is required."]
                            });
                    }

                    var scenario = HospitalSimulationScenarioCatalog.Find(request.ScenarioId);
                    if (scenario is null)
                    {
                        return TypedResults.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                [nameof(request.ScenarioId)] = ["Unknown scenario."]
                            });
                    }

                    var behavior = scenario.BehaviorOptions.FirstOrDefault(
                        option => string.Equals(
                            option.Id,
                            request.BehaviorOptionId,
                            StringComparison.OrdinalIgnoreCase));

                    if (behavior is null)
                    {
                        return TypedResults.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                [nameof(request.BehaviorOptionId)] =
                                ["Unknown behavior option for this scenario."]
                            });
                    }

                    await publisher.Publish(
                        new HospitalSimulationBehavior
                        {
                            SimulationId = request.SimulationId,
                            ScenarioName = scenario.Name,
                            StaffRole = scenario.StaffRole,
                            ObservedBehavior = behavior.Description,
                            ExpectedBehavior = scenario.ExpectedBehavior,
                            MetExpectedBehavior = behavior.MeetsExpectedBehavior,
                            ObservedAtUtc = DateTime.UtcNow
                        },
                        cancellationToken);

                    return TypedResults.Json(
                        new SimulationAcceptedResponse(request.SimulationId, "accepted"),
                        statusCode: StatusCodes.Status202Accepted);
                })
            .WithName("CreateSimulation")
            .WithSummary("Start a hospital simulation training assessment")
            .WithDescription(
                "Processes the selected behavior in the API through rubric assessment and AI reflection.")
            .Produces<SimulationAcceptedResponse>(StatusCodes.Status202Accepted)
            .ProducesValidationProblem();

        return endpoints;
    }
}
