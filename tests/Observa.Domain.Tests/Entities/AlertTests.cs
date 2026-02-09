using System;
using FluentAssertions;
using Observa.Domain.Entities;
using Observa.Domain.Enums;
using Observa.Domain.ValueObjects;
using Xunit;

namespace Observa.Domain.Tests.Entities;

public sealed class AlertTests
{
    [Fact]
    public void Create_ShouldCreateAlertWithCorrectData()
    {
        var rule = ThresholdRule.Create("cpu", 90.0, ThresholdOperator.GreaterThan, AlertSeverity.Critical).Value;
        var dashboardId = Guid.NewGuid();

        var alert = Alert.Create(dashboardId, rule, "CPU supero el 90%");

        alert.DashboardId.Should().Be(dashboardId);
        alert.Rule.Should().Be(rule);
        alert.Message.Should().Be("CPU supero el 90%");
        alert.IsAcknowledged.Should().BeFalse();
        alert.AcknowledgedAt.Should().BeNull();
    }

    [Fact]
    public void Acknowledge_ShouldMarkAsAcknowledged()
    {
        var rule = ThresholdRule.Create("memory", 95.0, ThresholdOperator.GreaterThan, AlertSeverity.Warning).Value;
        var alert = Alert.Create(Guid.NewGuid(), rule, "Memoria alta");

        alert.Acknowledge();

        alert.IsAcknowledged.Should().BeTrue();
        alert.AcknowledgedAt.Should().NotBeNull();
    }
}
