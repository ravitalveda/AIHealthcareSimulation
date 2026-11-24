using AIHealthcareSimulation.Contracts;
using MassTransit;
using System.Net.Http.Json;

namespace AIHealthcareSimulation.Api.Consumers;

public sealed class TrainingDebriefConsumer : IConsumer<TrainingAssessmentProcessed>
{
    private readonly HttpClient _httpClient;

    public TrainingDebriefConsumer(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task Consume(ConsumeContext<TrainingAssessmentProcessed> context)
    {
        var assessment = context.Message;
        var prompt = $"""
            Write a short explanation for an interviewer reviewing a hospital staff training simulation.
            The learner observed: "{assessment.ObservedBehavior}"
            The expected behavior was: "{assessment.ExpectedBehavior}"
            Scenario: "{assessment.ScenarioName}"

            Output exactly 2 plain-language sentences.
            Sentence 1 explains why the difference matters.
            Sentence 2 gives one practical next step.
            Address the learner directly.
            Do not repeat the supplied labels, use headings, bullets, markdown, or add a disclaimer.
            This is a training example, not care for a real patient.
            """;

        var request = new
        {
            model = "phi3",
            prompt,
            stream = false,
            options = new { temperature = 0, num_predict = 120 }
        };

        var response = await _httpClient.PostAsJsonAsync(
            "http://localhost:11434/api/generate",
            request,
            context.CancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(
            cancellationToken: context.CancellationToken)
            ?? throw new InvalidOperationException("Ollama returned an empty response.");

        if (string.IsNullOrWhiteSpace(result.Response))
            throw new InvalidOperationException("Ollama returned an empty reflection question.");

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

    private sealed class OllamaResponse
    {
        public string Response { get; set; } = string.Empty;
    }
}
