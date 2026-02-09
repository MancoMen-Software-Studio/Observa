using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Observa.Application.Commands.Dashboards;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Entities;
using Observa.Domain.Enums;
using Observa.Domain.Repositories;
using Observa.Domain.ValueObjects;
using Xunit;

namespace Observa.Application.Tests.Commands.Dashboards;

public sealed class PublishDashboardCommandHandlerTests
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PublishDashboardCommandHandler _handler;

    public PublishDashboardCommandHandlerTests()
    {
        _dashboardRepository = Substitute.For<IDashboardRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new PublishDashboardCommandHandler(_dashboardRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithWidgets_ShouldReturnSuccess()
    {
        var dashboard = CreateDashboardWithWidget();
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var command = new PublishDashboardCommand(dashboard.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        dashboard.Status.Should().Be(DashboardStatus.Published);
    }

    [Fact]
    public async Task Handle_WithoutWidgets_ShouldReturnFailure()
    {
        var dashboard = Dashboard.Create("Test", "Desc").Value;
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var command = new PublishDashboardCommand(dashboard.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.NoWidgets");
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnFailure()
    {
        _dashboardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Dashboard?)null);

        var command = new PublishDashboardCommand(Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.NotFound");
    }

    [Fact]
    public async Task Handle_AlreadyPublished_ShouldReturnFailure()
    {
        var dashboard = CreateDashboardWithWidget();
        dashboard.Publish();
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var command = new PublishDashboardCommand(dashboard.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.AlreadyPublished");
    }

    private static Dashboard CreateDashboardWithWidget()
    {
        var dashboard = Dashboard.Create("Test", "Desc").Value;
        var position = WidgetPosition.Create(0, 0, 4, 3).Value;
        var widget = Widget.Create("Widget", WidgetType.LineChart, position, Guid.NewGuid(), RefreshInterval.FiveSeconds).Value;
        dashboard.AddWidget(widget);
        return dashboard;
    }
}
