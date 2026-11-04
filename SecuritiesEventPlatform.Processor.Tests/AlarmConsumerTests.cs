namespace SecuritiesEventPlatform.Processor.Tests;

public class IncidentProcessingConsumerTests
{
    [Theory]
    [InlineData("SettlementFailure", "CRITICAL")]
    [InlineData("CorporateActionException", "CRITICAL")]
    [InlineData("CashReconciliationBreak", "HIGH")]
    [InlineData("TradeMatchingException", "HIGH")]
    [InlineData("CustodyInstructionFailure", "MEDIUM")]
    [InlineData("Unknown", "MEDIUM")]
    [InlineData("", "MEDIUM")]
    [InlineData(null, "MEDIUM")]
    public void AssessRisk_ReturnsExpectedRiskLevel(
        string? incidentType,
        string expectedRiskLevel)
    {
        // Act
        var actualRiskLevel = IncidentProcessingConsumer.AssessRisk(incidentType);

        // Assert
        Assert.Equal(expectedRiskLevel, actualRiskLevel);
    }

    [Theory]
    [InlineData("SettlementFailure", "CRITICAL")]
    [InlineData("CorporateActionException", "CRITICAL")]
    [InlineData("CashReconciliationBreak", "HIGH")]
    public void AssessRisk_ReturnsExpectedLevel_ForKnownIncidents(
        string incidentType,
        string expectedRiskLevel)
    {
        // Act
        var actualRiskLevel =
            IncidentProcessingConsumer.AssessRisk(incidentType);

        // Assert
        Assert.Equal(expectedRiskLevel, actualRiskLevel);
    }
}