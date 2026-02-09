using FluentAssertions;
using Observa.Domain.Enums;
using Observa.Domain.ValueObjects;
using Xunit;

namespace Observa.Domain.Tests.ValueObjects;

public sealed class ThresholdRuleTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccess()
    {
        var result = ThresholdRule.Create("cpu_usage", 90.0, ThresholdOperator.GreaterThan, AlertSeverity.Critical);

        result.IsSuccess.Should().BeTrue();
        result.Value.MetricName.Should().Be("cpu_usage");
        result.Value.Value.Should().Be(90.0);
    }

    [Fact]
    public void Create_WithEmptyMetricName_ShouldReturnFailure()
    {
        var result = ThresholdRule.Create("", 50.0, ThresholdOperator.GreaterThan, AlertSeverity.Warning);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("ThresholdRule.EmptyMetricName");
    }

    [Fact]
    public void IsTriggered_GreaterThan_WhenAboveThreshold_ShouldReturnTrue()
    {
        var rule = ThresholdRule.Create("cpu", 80.0, ThresholdOperator.GreaterThan, AlertSeverity.Warning).Value;

        rule.IsTriggered(85.0).Should().BeTrue();
    }

    [Fact]
    public void IsTriggered_GreaterThan_WhenBelowThreshold_ShouldReturnFalse()
    {
        var rule = ThresholdRule.Create("cpu", 80.0, ThresholdOperator.GreaterThan, AlertSeverity.Warning).Value;

        rule.IsTriggered(75.0).Should().BeFalse();
    }

    [Fact]
    public void IsTriggered_LessThan_WhenBelowThreshold_ShouldReturnTrue()
    {
        var rule = ThresholdRule.Create("memory", 20.0, ThresholdOperator.LessThan, AlertSeverity.Critical).Value;

        rule.IsTriggered(15.0).Should().BeTrue();
    }

    [Fact]
    public void IsTriggered_Equal_WhenMatchesThreshold_ShouldReturnTrue()
    {
        var rule = ThresholdRule.Create("status", 1.0, ThresholdOperator.Equal, AlertSeverity.Info).Value;

        rule.IsTriggered(1.0).Should().BeTrue();
    }
}
