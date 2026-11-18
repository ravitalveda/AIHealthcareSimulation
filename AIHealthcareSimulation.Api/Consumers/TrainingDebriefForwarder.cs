using AIHealthcareSimulation.Api.DTOs;
using AIHealthcareSimulation.Api.Hubs;
using AIHealthcareSimulation.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace AIHealthcareSimulation.Api.Consumers;

/// <summary>Forwards completed training debriefs to the matching SignalR group.</summary>
/// <param name="hubContext">The context used to send results to connected clients.</param>
public sealed class TrainingDebriefForwarder(
    IHubContext<TrainingResultsHub> hubContext)
    : IConsumer<TrainingDebriefCompleted>
{
    /// <summary>Forwards a completed debrief to the client subscribed to its simulation.</summary>
    public Task Consume(ConsumeContext<TrainingDebriefCompleted> context)
    {
        var debrief = context.Message;
        var response = new TrainingDebriefResponse(
            debrief.SimulationId,
            debrief.ScenarioName,
            debrief.StaffRole,
            debrief.ObservedBehavior,
            debrief.ExpectedBehavior,
            debrief.Outcome,
            debrief.AiReflectionQuestion,
            new DateTimeOffset(debrief.CompletedAtUtc));

        return hubContext.Clients
            .Group(debrief.SimulationId.ToString("D"))
            .SendAsync(
                "TrainingDebriefCompleted",
                response,
                context.CancellationToken);
    }
}
