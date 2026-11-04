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
                "Email & SMS Sent. Incident escalated to Operations Leadership and Support Team",

            "HIGH" =>
                "Email & SMS Sent. Operations team notified for immediate investigation",

            "MEDIUM" =>
                "Email sent. Incident added to operational review queue",

            _ =>
                "Notification/Incident logged for monitoring"
        };

        Console.WriteLine();
        Console.WriteLine("=================================================");
        Console.WriteLine("NOTIFICATION SERVICE");
        Console.WriteLine("=================================================");
        Console.WriteLine($"Incident Id   : {assessment.IncidentId}");
        Console.WriteLine($"Type          : {assessment.IncidentType}");
        Console.WriteLine($"Risk Level    : {assessment.RiskLevel}");
        Console.WriteLine($"Completed At  : {assessment.CompletedAtUtc:O}");
        Console.WriteLine();
        Console.WriteLine("Assessment by AI as:");
        Console.WriteLine(assessment.AiAssessment);
        Console.WriteLine();
        Console.WriteLine($"Action Taken  : {notificationAction}");
        Console.WriteLine("=================================================");
        Console.WriteLine();

        Console.ResetColor();

        return Task.CompletedTask;
    }
}