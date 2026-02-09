using System;
using FluentAssertions;
using Observa.Domain.Entities;
using Observa.Domain.Enums;
using Observa.Domain.ValueObjects;
using Xunit;

namespace Observa.Domain.Tests.Entities;

public sealed class WidgetTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccess()
    {
        var position = WidgetPosition.Create(0, 0, 4, 3).Value;

        var result = Widget.Create("Grafico CPU", WidgetType.LineChart, position, Guid.NewGuid(), RefreshInterval.FiveSeconds);

        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be("Grafico CPU");
        result.Value.Type.Should().Be(WidgetType.LineChart);
    }

    [Fact]
    public void Create_WithEmptyTitle_ShouldReturnFailure()
    {
        var position = WidgetPosition.Create(0, 0, 4, 3).Value;

        var result = Widget.Create("", WidgetType.BarChart, position, Guid.NewGuid(), RefreshInterval.OneMinute);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Widget.EmptyTitle");
    }

    [Fact]
    public void Create_WithTitleTooLong_ShouldReturnFailure()
    {
        var position = WidgetPosition.Create(0, 0, 4, 3).Value;
        var longTitle = new string('A', 101);

        var result = Widget.Create(longTitle, WidgetType.Gauge, position, Guid.NewGuid(), RefreshInterval.RealTime);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Widget.TitleTooLong");
    }

    [Fact]
    public void Create_WithEmptyDataSourceId_ShouldReturnFailure()
    {
        var position = WidgetPosition.Create(0, 0, 4, 3).Value;

        var result = Widget.Create("Widget", WidgetType.Table, position, Guid.Empty, RefreshInterval.FiveMinutes);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Widget.InvalidDataSource");
    }

    [Fact]
    public void UpdateTitle_WithValidTitle_ShouldUpdate()
    {
        var widget = CreateValidWidget();

        var result = widget.UpdateTitle("Nuevo titulo");

        result.IsSuccess.Should().BeTrue();
        widget.Title.Should().Be("Nuevo titulo");
    }

    [Fact]
    public void UpdatePosition_ShouldUpdatePosition()
    {
        var widget = CreateValidWidget();
        var newPosition = WidgetPosition.Create(5, 5, 6, 4).Value;

        widget.UpdatePosition(newPosition);

        widget.Position.Should().Be(newPosition);
    }

    private static Widget CreateValidWidget()
    {
        var position = WidgetPosition.Create(0, 0, 4, 3).Value;
        return Widget.Create("Test Widget", WidgetType.LineChart, position, Guid.NewGuid(), RefreshInterval.FiveSeconds).Value;
    }
}
