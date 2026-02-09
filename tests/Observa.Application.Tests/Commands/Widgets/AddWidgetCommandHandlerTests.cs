using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Observa.Application.Commands.Widgets;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Enums;
using Observa.Domain.Repositories;
using Xunit;

namespace Observa.Application.Tests.Commands.Widgets;

public sealed class AddWidgetCommandHandlerTests
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AddWidgetCommandHandler _handler;

    public AddWidgetCommandHandlerTests()
    {
        _dashboardRepository = Substitute.For<IDashboardRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new AddWidgetCommandHandler(_dashboardRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldReturnSuccessWithWidgetId()
    {
        var dashboard = Dashboard.Create("Test", "Desc").Value;
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var command = new AddWidgetCommand(
            dashboard.Id, "Grafico CPU", WidgetType.LineChart,
            0, 0, 4, 3, Guid.NewGuid(), RefreshInterval.FiveSeconds);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        dashboard.Widgets.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_WhenDashboardNotFound_ShouldReturnFailure()
    {
        _dashboardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Dashboard?)null);

        var command = new AddWidgetCommand(
            Guid.NewGuid(), "Widget", WidgetType.BarChart,
            0, 0, 4, 3, Guid.NewGuid(), RefreshInterval.OneMinute);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.NotFound");
    }

    [Fact]
    public async Task Handle_WithInvalidPosition_ShouldReturnFailure()
    {
        var dashboard = Dashboard.Create("Test", "Desc").Value;
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var command = new AddWidgetCommand(
            dashboard.Id, "Widget", WidgetType.Gauge,
            -1, 0, 4, 3, Guid.NewGuid(), RefreshInterval.RealTime);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("WidgetPosition.NegativePosition");
    }

    [Fact]
    public async Task Handle_WithEmptyTitle_ShouldReturnFailure()
    {
        var dashboard = Dashboard.Create("Test", "Desc").Value;
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var command = new AddWidgetCommand(
            dashboard.Id, "", WidgetType.Table,
            0, 0, 4, 3, Guid.NewGuid(), RefreshInterval.FiveMinutes);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Widget.EmptyTitle");
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCallUpdateAndSave()
    {
        var dashboard = Dashboard.Create("Test", "Desc").Value;
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var command = new AddWidgetCommand(
            dashboard.Id, "Widget", WidgetType.LineChart,
            0, 0, 4, 3, Guid.NewGuid(), RefreshInterval.FiveSeconds);

        await _handler.Handle(command, CancellationToken.None);

        _dashboardRepository.Received(1).Update(dashboard);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
