using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Observa.Application.Abstractions.Notifications;
using Observa.Application.Commands.Dashboards;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Repositories;
using Xunit;

namespace Observa.Application.Tests.Commands.Dashboards;

public sealed class CreateDashboardCommandHandlerTests
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDashboardNotificationService _notificationService;
    private readonly CreateDashboardCommandHandler _handler;

    public CreateDashboardCommandHandlerTests()
    {
        _dashboardRepository = Substitute.For<IDashboardRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _notificationService = Substitute.For<IDashboardNotificationService>();
        _handler = new CreateDashboardCommandHandler(_dashboardRepository, _unitOfWork, _notificationService);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldReturnSuccessWithId()
    {
        var command = new CreateDashboardCommand("Mi Dashboard", "Descripcion");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCallAddAndSave()
    {
        var command = new CreateDashboardCommand("Mi Dashboard", "Descripcion");

        await _handler.Handle(command, CancellationToken.None);

        await _dashboardRepository.Received(1).AddAsync(Arg.Any<Dashboard>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithEmptyTitle_ShouldReturnFailure()
    {
        var command = new CreateDashboardCommand("", "Descripcion");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.EmptyTitle");
    }

    [Fact]
    public async Task Handle_WithEmptyTitle_ShouldNotCallRepository()
    {
        var command = new CreateDashboardCommand("", "Descripcion");

        await _handler.Handle(command, CancellationToken.None);

        await _dashboardRepository.DidNotReceive().AddAsync(Arg.Any<Dashboard>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
