using AIHealthcareSimulation.Contracts;

namespace AIHealthcareSimulation.Tests;

public class HospitalSimulationScenarioCatalogTests
{
    [Fact]
    public void Find_ReturnsScenarioWhenIdDiffersOnlyByCase()
    {
        var scenario = HospitalSimulationScenarioCatalog.All[0];

        var found = HospitalSimulationScenarioCatalog.Find(
            scenario.Id.ToUpperInvariant());

        Assert.Equal(scenario, found);
    }

    [Fact]
    public void Find_ReturnsNullForUnknownId()
    {
        var found = HospitalSimulationScenarioCatalog.Find("not-a-scenario");

        Assert.Null(found);
    }

    [Fact]
    public void AllScenarios_HaveUniqueIdsAndConsistentExpectedBehavior()
    {
        var scenarios = HospitalSimulationScenarioCatalog.All;

        Assert.NotEmpty(scenarios);
        Assert.Equal(
            scenarios.Count,
            scenarios.Select(scenario => scenario.Id)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count());

        foreach (var scenario in scenarios)
        {
            Assert.NotEmpty(scenario.BehaviorOptions);
            Assert.Equal(
                scenario.BehaviorOptions.Count,
                scenario.BehaviorOptions.Select(option => option.Id)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count());

            var expectedBehavior = Assert.Single(
                scenario.BehaviorOptions,
                option => option.MeetsExpectedBehavior);

            Assert.Equal(scenario.ExpectedBehavior, expectedBehavior.Description);
            Assert.Contains(
                scenario.BehaviorOptions,
                option => !option.MeetsExpectedBehavior);
        }
    }
}
