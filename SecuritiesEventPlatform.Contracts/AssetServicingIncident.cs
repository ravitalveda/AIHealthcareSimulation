namespace SecuritiesEventPlatform.Contracts;

public record AssetServicingIncident
{
    public Guid IncidentId { get; init; }

    public string AccountId { get; init; } = string.Empty;

    public string ClientId { get; init; } = string.Empty;

    public string IncidentType { get; init; } = string.Empty;

    public DateTime OccurredAtUtc { get; init; }
}
