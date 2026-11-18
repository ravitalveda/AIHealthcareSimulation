using AIHealthcareSimulation.Contracts;

namespace AIHealthcareSimulation.Api.DTOs;

/// <summary>Contains the rubric result and AI-generated reflection question.</summary>
public sealed record TrainingDebriefResponse(
    Guid SimulationId,
    string ScenarioName,
    string StaffRole,
    string ObservedBehavior,
    string ExpectedBehavior,
    TrainingOutcome Outcome,
    string AiReflectionQuestion,
    DateTimeOffset CompletedAt);
