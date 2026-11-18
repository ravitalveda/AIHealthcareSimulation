using MassTransit;
using AIHealthcareSimulation.Contracts;

namespace AIHealthcareSimulation.Notification;

public class TrainingNotificationConsumer : IConsumer<TrainingDebriefCompleted>
{
    public Task Consume(ConsumeContext<TrainingDebriefCompleted> context)
    {
        var debrief = context.Message;
        var followUp = debrief.Outcome switch
        {
            TrainingOutcome.MeetsExpectations =>
                "Recognize the demonstrated behavior and continue to the next learning objective.",
            TrainingOutcome.PracticeRecommended =>
                "Review the practice opportunity with an educator and repeat the simulation.",
            _ => "Review the simulation outcome with an educator."
        };

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine();
        Console.WriteLine("=================================================");
        Console.WriteLine("SIMULATION TRAINING FEEDBACK");
        Console.WriteLine("=================================================");
        Console.WriteLine($"Simulation Id : {debrief.SimulationId}");
        Console.WriteLine($"Scenario      : {debrief.ScenarioName}");
        Console.WriteLine($"Staff Role    : {debrief.StaffRole}");
        Console.WriteLine($"Outcome       : {debrief.Outcome}");
        Console.WriteLine($"Completed At  : {debrief.CompletedAtUtc:O}");
        Console.WriteLine();
        Console.WriteLine("AI reflection question (draft):");
        Console.WriteLine(debrief.AiReflectionQuestion);
        Console.WriteLine();
        Console.WriteLine($"Suggested follow-up: {followUp}");
        Console.WriteLine("=================================================");
        Console.ResetColor();

        return Task.CompletedTask;
    }
}
