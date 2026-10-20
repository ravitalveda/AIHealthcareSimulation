using MassTransit;
using SecuritiesEventPlatform.Contracts;

var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost");
});

await bus.StartAsync();

Console.WriteLine("Securities Event Publisher Started");
Console.WriteLine("Publishing simulated operational incidents");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1 - Settlement Failure");
    Console.WriteLine("2 - Corporate Action Exception");
    Console.WriteLine("3 - Cash Reconciliation Break");
    Console.WriteLine("4 - Trade Matching Exception");
    Console.WriteLine("5 - Custody Instruction Failure");
    Console.WriteLine("Q - Quit");

    var input = Console.ReadLine();

    if (string.Equals(input, "Q", StringComparison.OrdinalIgnoreCase))
        break;

    var incidentType = input switch
    {
        "1" => "SettlementFailure",
        "2" => "CorporateActionException",
        "3" => "CashReconciliationBreak",
        "4" => "TradeMatchingException",
        "5" => "CustodyInstructionFailure",
        _ => null
    };

    if (incidentType is null)
    {
        Console.WriteLine("Invalid selection.");
        continue;
    }

    await bus.Publish(new AssetServicingIncident
    {
        IncidentId = Guid.NewGuid(),
        AccountId = $"ACC-{Random.Shared.Next(1000, 9999)}",
        ClientId = $"CLIENT-{Random.Shared.Next(100, 999)}",
        IncidentType = incidentType,
        OccurredAtUtc = DateTime.UtcNow
    });

    Console.WriteLine($"Published Incident: {incidentType}");
}

await bus.StopAsync();

Console.WriteLine();
Console.WriteLine("Publisher stopped.");