using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Observa.Application.Abstractions.Notifications;
using Observa.Application.Commands.Dashboards;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Enums;
using Observa.Domain.Repositories;
using Xunit;

namespace Observa.Application.Tests.Commands.Dashboards;

public sealed class ArchiveDashboardCommandHandlerTests
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDashboardNotificationService _notificationService;
    private readonly ArchiveDashboardCommandHandler _handler;

    public ArchiveDashboardCommandHandlerTests()
    {
        _dashboardRepository = Substitute.For<IDashboardRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _notificationService = Substitute.For<IDashboardNotificationService>();
        _handler = new ArchiveDashboardCommandHandler(_dashboardRepository, _unitOfWork, _notificationService);
    }

    [Fact]
    public async Task Handle_WithValidDashboard_ShouldReturnSuccess()
    {
        var dashboard = Dashboard.Create("Test", "Desc").Value;
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var command = new ArchiveDashboardCommand(dashboard.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        dashboard.Status.Should().Be(DashboardStatus.Archived);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnFailure()
    {
        _dashboardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Dashboard?)null);

        var command = new ArchiveDashboardCommand(Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.NotFound");
    }

    [Fact]
    public async Task Handle_AlreadyArchived_ShouldReturnFailure()
    {
        var dashboard = Dashboard.Create("Test", "Desc").Value;
        dashboard.Archive();
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var command = new ArchiveDashboardCommand(dashboard.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.AlreadyArchived");
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldNotCallSave()
    {
        _dashboardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Dashboard?)null);

        var command = new ArchiveDashboardCommand(Guid.NewGuid());

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
