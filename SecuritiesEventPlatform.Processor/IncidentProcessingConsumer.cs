using MassTransit;
using SecuritiesEventPlatform.Contracts;

namespace SecuritiesEventPlatform.Processor;

public class IncidentProcessingConsumer : IConsumer<AssetServicingIncident>
{
    public static string AssessRisk(string? incidentType)
    {

        return incidentType switch
        {
            "SettlementFailure" => "CRITICAL",
            "CorporateActionException" => "CRITICAL",
            "CashReconciliationBreak" => "HIGH",
            "TradeMatchingException" => "HIGH",
            "CustodyInstructionFailure" => "MEDIUM",
            _ => "MEDIUM"
        };
    }

    public async Task Consume(ConsumeContext<AssetServicingIncident> context)
    {
        var incident = context.Message;

        var riskLevel = AssessRisk(incident.IncidentType);

        Console.ForegroundColor = riskLevel switch
        {
            "CRITICAL" or "HIGH" => ConsoleColor.Red,
            "MEDIUM" => ConsoleColor.Yellow,
            _ => ConsoleColor.Green
        };

        Console.WriteLine();
        Console.WriteLine("=================================================");
        Console.WriteLine("SECURITIES EVENT PROCESSOR");
        Console.WriteLine("=================================================");
        Console.WriteLine($"Processed At : {DateTime.UtcNow:O}");
        Console.WriteLine($"Incident Id  : {incident.IncidentId}");
        Console.WriteLine($"Client Id    : {incident.ClientId}");
        Console.WriteLine($"Account Id   : {incident.AccountId}");
        Console.WriteLine($"Type         : {incident.IncidentType}");
        Console.WriteLine($"Risk Level   : {riskLevel}");
        Console.WriteLine("=================================================");


        await context.Publish(new RiskAssessmentProcessed
        {
            IncidentId = incident.IncidentId,
            IncidentType = incident.IncidentType,
            RiskLevel = riskLevel,
            AssessedAtUtc = DateTime.UtcNow
        });

        Console.WriteLine("Risk assessment published.");

        Console.ResetColor();
    }
}
