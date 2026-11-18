using Microsoft.AspNetCore.SignalR;

namespace AIHealthcareSimulation.Api.Hubs;

/// <summary>Provides real-time delivery of asynchronous training results.</summary>
public sealed class TrainingResultsHub : Hub
{
    /// <summary>Subscribes the current connection to results for one simulation.</summary>
    /// <param name="simulationId">The simulation ID to receive results for.</param>
    /// <returns>A task that completes when the connection joins the result group.</returns>
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
