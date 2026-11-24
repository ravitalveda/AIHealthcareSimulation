using AIHealthcareSimulation.Contracts;
using AIHealthcareSimulation.Api.Consumers;

namespace AIHealthcareSimulation.Tests;

public class TrainingAssessmentTests
{
    [Theory]
    [InlineData(true, TrainingOutcome.MeetsExpectations)]
    [InlineData(false, TrainingOutcome.PracticeRecommended)]
    public void AssessOutcome_MapsRubricResult(
        bool metExpectedBehavior,
        TrainingOutcome expectedOutcome)
    {
        var actualOutcome =
            TrainingAssessmentConsumer.AssessOutcome(metExpectedBehavior);

        Assert.Equal(expectedOutcome, actualOutcome);
    }
}
