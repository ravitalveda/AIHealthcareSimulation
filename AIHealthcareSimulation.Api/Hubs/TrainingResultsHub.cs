using Microsoft.AspNetCore.SignalR;

namespace AIHealthcareSimulation.Api.Hubs;

/// <summary>Provides real-time delivery of asynchronous training results.</summary>
public sealed class TrainingResultsHub : Hub
{
    public Task JoinSimulation(Guid simulationId)
    {
        if (simulationId == Guid.Empty)
        {
            throw new HubException("A non-empty simulation ID is required.");
        }

        return Groups.AddToGroupAsync(
            Context.ConnectionId,
            simulationId.ToString("D"),
            Context.ConnectionAborted);
    }
}
