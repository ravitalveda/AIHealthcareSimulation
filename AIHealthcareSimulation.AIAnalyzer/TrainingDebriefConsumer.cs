using MassTransit;
using AIHealthcareSimulation.Contracts;
using System.Net.Http.Json;

namespace AIHealthcareSimulation.AIAnalyzer;

public sealed class TrainingDebriefConsumer : IConsumer<TrainingAssessmentProcessed>
{
    private readonly HttpClient _httpClient;

    public TrainingDebriefConsumer(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task Consume(ConsumeContext<TrainingAssessmentProcessed> context)
    {
        try
        {
            var assessment = context.Message;

            var prompt = $"""
                You are supporting educator-led training in a simulated hospital scenario.
                This is a training exercise, not care for a real patient.
                Return exactly one complete question and no other text.
                Copy the supplied values exactly into this template. Do not summarize or add information:

                How does the observed behavior "{assessment.ObservedBehavior}" compare with the expected behavior "{assessment.ExpectedBehavior}" in the "{assessment.ScenarioName}" scenario?
                """;

            var request = new
            {
                model = "phi3",
                prompt,
                stream = false,
                options = new { temperature = 0 }
            };

            var response = await _httpClient.PostAsJsonAsync(
                "http://localhost:11434/api/generate",
                request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>()
                ?? throw new InvalidOperationException("Ollama returned an empty response.");

            if (string.IsNullOrWhiteSpace(result.Response))
                throw new InvalidOperationException("Ollama returned an empty reflection question.");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("=================================================");
            Console.WriteLine("AI TRAINING REFLECTION QUESTION");
            Console.WriteLine("=================================================");
            Console.WriteLine($"Simulation Id : {assessment.SimulationId}");
            Console.WriteLine($"Scenario      : {assessment.ScenarioName}");
            Console.WriteLine($"Staff Role    : {assessment.StaffRole}");
            Console.WriteLine($"Outcome       : {assessment.Outcome}");
            Console.WriteLine();
            Console.WriteLine(result.Response);
            Console.WriteLine("=================================================");
            Console.ResetColor();

            await context.Publish(new TrainingDebriefCompleted
            {
                SimulationId = assessment.SimulationId,
                ScenarioName = assessment.ScenarioName,
                StaffRole = assessment.StaffRole,
                ObservedBehavior = assessment.ObservedBehavior,
                ExpectedBehavior = assessment.ExpectedBehavior,
                Outcome = assessment.Outcome,
                AiReflectionQuestion = result.Response,
                CompletedAtUtc = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(
                $"Training reflection question failed for simulation {context.Message.SimulationId}: {ex}");
            throw;
        }
    }
}
