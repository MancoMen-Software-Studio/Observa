using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Observa.Application.Commands.Widgets;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Entities;
using Observa.Domain.Enums;
using Observa.Domain.Repositories;
using Observa.Domain.ValueObjects;
using Xunit;

namespace Observa.Application.Tests.Commands.Widgets;

public sealed class RemoveWidgetCommandHandlerTests
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RemoveWidgetCommandHandler _handler;

    public RemoveWidgetCommandHandlerTests()
    {
        _dashboardRepository = Substitute.For<IDashboardRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new RemoveWidgetCommandHandler(_dashboardRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithExistingWidget_ShouldReturnSuccess()
    {
        var dashboard = Dashboard.Create("Test", "Desc").Value;
        var widget = CreateValidWidget();
        dashboard.AddWidget(widget);
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var command = new RemoveWidgetCommand(dashboard.Id, widget.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        dashboard.Widgets.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenDashboardNotFound_ShouldReturnFailure()
    {
        _dashboardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Dashboard?)null);

        var command = new RemoveWidgetCommand(Guid.NewGuid(), Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.NotFound");
    }

    [Fact]
    public async Task Handle_WithNonExistingWidget_ShouldReturnFailure()
    {
        var dashboard = Dashboard.Create("Test", "Desc").Value;
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var command = new RemoveWidgetCommand(dashboard.Id, Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Widget.NotFound");
    }

    [Fact]
    public async Task Handle_WithExistingWidget_ShouldCallUpdateAndSave()
    {
        var dashboard = Dashboard.Create("Test", "Desc").Value;
        var widget = CreateValidWidget();
        dashboard.AddWidget(widget);
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var command = new RemoveWidgetCommand(dashboard.Id, widget.Id);

        await _handler.Handle(command, CancellationToken.None);

        _dashboardRepository.Received(1).Update(dashboard);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static Widget CreateValidWidget()
    {
        var position = WidgetPosition.Create(0, 0, 4, 3).Value;
        return Widget.Create("Widget", WidgetType.LineChart, position, Guid.NewGuid(), RefreshInterval.FiveSeconds).Value;
    }
}
