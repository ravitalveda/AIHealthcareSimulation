using MassTransit;
using SecuritiesEventPlatform.Contracts;

namespace SecuritiesEventPlatform.Notification;

public class IncidentNotificationConsumer : IConsumer<AIAssessmentCompleted>
{
    public Task Consume(ConsumeContext<AIAssessmentCompleted> context)
    {
        var assessment = context.Message;

        Console.ForegroundColor = ConsoleColor.Yellow;

        var notificationAction = assessment.RiskLevel switch
        {
            "CRITICAL" =>
                "Incident escalated to Operations Leadership and Support Team",

            "HIGH" =>
                "Operations team notified for immediate investigation",

            "MEDIUM" =>
                "Incident added to operational review queue",

            _ =>
                "Incident logged for monitoring"
        };

        Console.WriteLine();
        Console.WriteLine("=================================================");
        Console.WriteLine("INCIDENT NOTIFICATION SERVICE");
        Console.WriteLine("=================================================");
        Console.WriteLine($"Incident Id   : {assessment.IncidentId}");
        Console.WriteLine($"Type          : {assessment.IncidentType}");
        Console.WriteLine($"Risk Level    : {assessment.RiskLevel}");
        Console.WriteLine($"Completed At  : {assessment.CompletedAtUtc:O}");
        Console.WriteLine();
        Console.WriteLine("AI Assessment:");
        Console.WriteLine(assessment.AiAssessment);
        Console.WriteLine();
        Console.WriteLine($"Action Taken  : {notificationAction}");
        Console.WriteLine("=================================================");
        Console.WriteLine();

        Console.ResetColor();

        return Task.CompletedTask;
    }
}