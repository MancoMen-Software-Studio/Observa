using System;
using System.Linq;
using FluentAssertions;
using Observa.Domain.Aggregates;
using Observa.Domain.Entities;
using Observa.Domain.Enums;
using Observa.Domain.Events;
using Observa.Domain.ValueObjects;
using Xunit;

namespace Observa.Domain.Tests.Aggregates;

public sealed class DashboardTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccess()
    {
        var result = Dashboard.Create("Mi Dashboard", "Descripcion de prueba");

        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be("Mi Dashboard");
        result.Value.Description.Should().Be("Descripcion de prueba");
        result.Value.Status.Should().Be(DashboardStatus.Draft);
    }

    [Fact]
    public void Create_ShouldRaiseDashboardCreatedEvent()
    {
        var result = Dashboard.Create("Dashboard Test", "Desc");

        result.Value.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<DashboardCreatedEvent>();
    }

    [Fact]
    public void Create_WithEmptyTitle_ShouldReturnFailure()
    {
        var result = Dashboard.Create("", "Desc");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.EmptyTitle");
    }

    [Fact]
    public void Create_WithTitleTooLong_ShouldReturnFailure()
    {
        var longTitle = new string('A', 201);

        var result = Dashboard.Create(longTitle, "Desc");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.TitleTooLong");
    }

    [Fact]
    public void AddWidget_ShouldAddWidgetAndRaiseEvent()
    {
        var dashboard = CreateValidDashboard();
        var widget = CreateValidWidget();

        var result = dashboard.AddWidget(widget);

        result.IsSuccess.Should().BeTrue();
        dashboard.Widgets.Should().ContainSingle();
    }

    [Fact]
    public void AddWidget_WhenMaxReached_ShouldReturnFailure()
    {
        var dashboard = CreateValidDashboard();

        for (int i = 0; i < 20; i++)
        {
            dashboard.AddWidget(CreateValidWidget());
        }

        var result = dashboard.AddWidget(CreateValidWidget());

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.MaxWidgetsReached");
    }

    [Fact]
    public void AddWidget_Duplicate_ShouldReturnFailure()
    {
        var dashboard = CreateValidDashboard();
        var widget = CreateValidWidget();
        dashboard.AddWidget(widget);

        var result = dashboard.AddWidget(widget);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.DuplicateWidget");
    }

    [Fact]
    public void RemoveWidget_Existing_ShouldRemoveAndRaiseEvent()
    {
        var dashboard = CreateValidDashboard();
        var widget = CreateValidWidget();
        dashboard.AddWidget(widget);

        var result = dashboard.RemoveWidget(widget.Id);

        result.IsSuccess.Should().BeTrue();
        dashboard.Widgets.Should().BeEmpty();
    }

    [Fact]
    public void RemoveWidget_NonExisting_ShouldReturnFailure()
    {
        var dashboard = CreateValidDashboard();

        var result = dashboard.RemoveWidget(Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Widget.NotFound");
    }

    [Fact]
    public void Publish_WithWidgets_ShouldChangeStatus()
    {
        var dashboard = CreateValidDashboard();
        dashboard.AddWidget(CreateValidWidget());

        var result = dashboard.Publish();

        result.IsSuccess.Should().BeTrue();
        dashboard.Status.Should().Be(DashboardStatus.Published);
    }

    [Fact]
    public void Publish_WithoutWidgets_ShouldReturnFailure()
    {
        var dashboard = CreateValidDashboard();

        var result = dashboard.Publish();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.NoWidgets");
    }

    [Fact]
    public void Publish_AlreadyPublished_ShouldReturnFailure()
    {
        var dashboard = CreateValidDashboard();
        dashboard.AddWidget(CreateValidWidget());
        dashboard.Publish();

        var result = dashboard.Publish();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.AlreadyPublished");
    }

    [Fact]
    public void Archive_ShouldChangeStatus()
    {
        var dashboard = CreateValidDashboard();

        var result = dashboard.Archive();

        result.IsSuccess.Should().BeTrue();
        dashboard.Status.Should().Be(DashboardStatus.Archived);
    }

    [Fact]
    public void Archive_AlreadyArchived_ShouldReturnFailure()
    {
        var dashboard = CreateValidDashboard();
        dashboard.Archive();

        var result = dashboard.Archive();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.AlreadyArchived");
    }

    [Fact]
    public void UpdateTitle_WithValidTitle_ShouldUpdateTitle()
    {
        var dashboard = CreateValidDashboard();

        var result = dashboard.UpdateTitle("Nuevo Titulo");

        result.IsSuccess.Should().BeTrue();
        dashboard.Title.Should().Be("Nuevo Titulo");
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription()
    {
        var dashboard = CreateValidDashboard();

        var result = dashboard.UpdateDescription("Nueva descripcion");

        result.IsSuccess.Should().BeTrue();
        dashboard.Description.Should().Be("Nueva descripcion");
    }

    private static Dashboard CreateValidDashboard()
    {
        return Dashboard.Create("Test Dashboard", "Test Description").Value;
    }

    private static Widget CreateValidWidget()
    {
        var position = WidgetPosition.Create(0, 0, 4, 3).Value;
        return Widget.Create("Widget Test", WidgetType.LineChart, position, Guid.NewGuid(), RefreshInterval.FiveSeconds).Value;
    }
}
