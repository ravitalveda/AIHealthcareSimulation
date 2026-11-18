using MassTransit;
using AIHealthcareSimulation.Contracts;

namespace AIHealthcareSimulation.Processor;

public class TrainingAssessmentConsumer : IConsumer<HospitalSimulationBehavior>
{
    public static TrainingOutcome AssessOutcome(bool metExpectedBehavior) =>
        metExpectedBehavior
            ? TrainingOutcome.MeetsExpectations
            : TrainingOutcome.PracticeRecommended;

    public async Task Consume(ConsumeContext<HospitalSimulationBehavior> context)
    {
        var behavior = context.Message;
        var outcome = AssessOutcome(behavior.MetExpectedBehavior);

        Console.WriteLine();
        Console.WriteLine("=================================================");
        Console.WriteLine("HOSPITAL SIMULATION TRAINING ASSESSMENT");
        Console.WriteLine("=================================================");
        Console.WriteLine($"Simulation Id : {behavior.SimulationId}");
        Console.WriteLine($"Scenario      : {behavior.ScenarioName}");
        Console.WriteLine($"Staff Role    : {behavior.StaffRole}");
        Console.WriteLine($"Observed      : {behavior.ObservedBehavior}");
        Console.WriteLine($"Outcome       : {outcome}");
        Console.WriteLine("=================================================");

        await context.Publish(new TrainingAssessmentProcessed
        {
            SimulationId = behavior.SimulationId,
            ScenarioName = behavior.ScenarioName,
            StaffRole = behavior.StaffRole,
            ObservedBehavior = behavior.ObservedBehavior,
            ExpectedBehavior = behavior.ExpectedBehavior,
            Outcome = outcome,
            AssessedAtUtc = DateTime.UtcNow
        });
    }
}
