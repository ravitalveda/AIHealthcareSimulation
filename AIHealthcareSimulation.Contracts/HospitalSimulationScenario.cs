namespace AIHealthcareSimulation.Contracts;

public sealed record SimulatedBehaviorOption(
    string Id,
    string Description,
    bool MeetsExpectedBehavior);

public sealed record HospitalSimulationScenario(
    string Id,
    string Name,
    string StaffRole,
    string ExpectedBehavior,
    IReadOnlyList<SimulatedBehaviorOption> BehaviorOptions);

public static class HospitalSimulationScenarioCatalog
{
    private static readonly IReadOnlyList<HospitalSimulationScenario> Scenarios =
    [
        new(
            "patient-identification",
            "Patient identification",
            "Nurse",
            "Verified identity using two approved identifiers",
            [
                new("expected", "Verified identity using two approved identifiers", true),
                new("practice", "Did not verify identity before proceeding", false)
            ]),
        new(
            "structured-handover",
            "Structured handover",
            "Nurse",
            "Communicated key patient information, concerns, and next steps",
            [
                new(
                    "expected",
                    "Communicated key patient information, concerns, and next steps",
                    true),
                new(
                    "practice",
                    "Omitted key patient information, concerns, or next steps",
                    false)
            ]),
        new(
            "escalation",
            "Escalating a simulated change in condition",
            "Nurse",
            "Escalated the simulated change promptly according to the scenario protocol",
            [
                new(
                    "expected",
                    "Escalated the simulated change promptly according to the scenario protocol",
                    true),
                new(
                    "practice",
                    "Did not escalate the simulated change promptly",
                    false)
            ])
    ];

    public static IReadOnlyList<HospitalSimulationScenario> All => Scenarios;

    public static HospitalSimulationScenario? Find(string id) =>
        Scenarios.FirstOrDefault(
            scenario => string.Equals(scenario.Id, id, StringComparison.OrdinalIgnoreCase));
}
