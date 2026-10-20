using MassTransit;
using SecuritiesEventPlatform.Contracts;
using System.Net.Http.Json;

namespace SecuritiesEventPlatform.AIAnalyzer;

public sealed class AIAssessmentConsumer : IConsumer<RiskAssessmentProcessed>
{
    private readonly HttpClient _httpClient;

    public AIAssessmentConsumer(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task Consume(ConsumeContext<RiskAssessmentProcessed> context)
    {
        try
        {
            var assessment = context.Message;

            var prompt = $"""
                You are an Asset Servicing Operations Analyst working in a Securities Services organization.

                Analyze the following operational incident.

                Incident Type: {assessment.IncidentType}
                Risk Level: {assessment.RiskLevel}

                Provide your response in the following format:

                Business Impact:
                Confidence:
                Recommended Action:
                """;

            var request = new
            {
                model = "phi3",
                prompt,
                stream = false
            };

            var response = await _httpClient.PostAsJsonAsync(
                "http://localhost:11434/api/generate",
                request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine();
            Console.WriteLine("=================================================");
            Console.WriteLine("AI RISK ASSESSMENT");
            Console.WriteLine("=================================================");
            Console.WriteLine($"Incident Id  : {assessment.IncidentId}");
            Console.WriteLine($"Type         : {assessment.IncidentType}");
            Console.WriteLine($"Risk Level   : {assessment.RiskLevel}");
            Console.WriteLine($"Assessed At  : {assessment.AssessedAtUtc:O}");
            Console.WriteLine();
            Console.WriteLine(result?.Response);
            Console.WriteLine("=================================================");

            Console.ResetColor();


            await context.Publish(new AIAssessmentCompleted
            {
                IncidentId = assessment.IncidentId,
                IncidentType = assessment.IncidentType,
                RiskLevel = assessment.RiskLevel,
                AiAssessment = result?.Response ?? string.Empty,
                CompletedAtUtc = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"AI analysis failed for incident {context.Message.IncidentId}: {ex}");
            throw;
        }
    }
}