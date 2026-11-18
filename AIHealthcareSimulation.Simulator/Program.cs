using MassTransit;
using AIHealthcareSimulation.Contracts;

var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost");
});

await bus.StartAsync();

Console.WriteLine("Hospital Simulation Training Publisher Started");
Console.WriteLine("Training-only examples; do not enter real patient information.");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Choose a simulated hospital scenario:");
    for (var index = 0; index < HospitalSimulationScenarioCatalog.All.Count; index++)
    {
        Console.WriteLine(
            $"{index + 1} - {HospitalSimulationScenarioCatalog.All[index].Name}");
    }

    Console.WriteLine("Q - Quit");

    var input = Console.ReadLine();

    if (string.Equals(input, "Q", StringComparison.OrdinalIgnoreCase))
        break;

    if (!int.TryParse(input, out var scenarioNumber)
        || scenarioNumber < 1
        || scenarioNumber > HospitalSimulationScenarioCatalog.All.Count)
    {
        Console.WriteLine("Invalid selection.");
        continue;
    }

    var scenario = HospitalSimulationScenarioCatalog.All[scenarioNumber - 1];
    Console.WriteLine();
    Console.WriteLine("Choose the simulated staff behavior:");
    for (var index = 0; index < scenario.BehaviorOptions.Count; index++)
    {
        Console.WriteLine(
            $"{index + 1} - {scenario.BehaviorOptions[index].Description}");
    }

    Console.WriteLine("Q - Return to scenarios");

    var behaviorInput = Console.ReadLine();
    if (string.Equals(behaviorInput, "Q", StringComparison.OrdinalIgnoreCase))
        continue;

    if (!int.TryParse(behaviorInput, out var behaviorNumber)
        || behaviorNumber < 1
        || behaviorNumber > scenario.BehaviorOptions.Count)
    {
        Console.WriteLine("Invalid selection.");
        continue;
    }

    var selectedBehavior = scenario.BehaviorOptions[behaviorNumber - 1];
    var behavior = new HospitalSimulationBehavior
    {
        SimulationId = Guid.NewGuid(),
        ScenarioName = scenario.Name,
        StaffRole = scenario.StaffRole,
        ObservedBehavior = selectedBehavior.Description,
        ExpectedBehavior = scenario.ExpectedBehavior,
        MetExpectedBehavior = selectedBehavior.MeetsExpectedBehavior,
        ObservedAtUtc = DateTime.UtcNow
    };

    await bus.Publish(behavior);

    Console.WriteLine($"Published simulation behavior: {behavior.ScenarioName}");
}

await bus.StopAsync();

Console.WriteLine();
Console.WriteLine("Publisher stopped.");
