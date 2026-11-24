using AIHealthcareSimulation.Contracts;
using MassTransit;

namespace AIHealthcareSimulation.Api.Consumers;

public sealed class TrainingAssessmentConsumer : IConsumer<HospitalSimulationBehavior>
{
    public static TrainingOutcome AssessOutcome(bool metExpectedBehavior) =>
        metExpectedBehavior
            ? TrainingOutcome.MeetsExpectations
            : TrainingOutcome.PracticeRecommended;

    public async Task Consume(ConsumeContext<HospitalSimulationBehavior> context)
    {
        var behavior = context.Message;
        var outcome = AssessOutcome(behavior.MetExpectedBehavior);

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
