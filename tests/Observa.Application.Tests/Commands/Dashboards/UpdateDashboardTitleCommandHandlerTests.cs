using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Observa.Application.Commands.Dashboards;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Repositories;
using Xunit;

namespace Observa.Application.Tests.Commands.Dashboards;

public sealed class UpdateDashboardTitleCommandHandlerTests
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdateDashboardTitleCommandHandler _handler;

    public UpdateDashboardTitleCommandHandlerTests()
    {
        _dashboardRepository = Substitute.For<IDashboardRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new UpdateDashboardTitleCommandHandler(_dashboardRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldReturnSuccess()
    {
        var dashboard = Dashboard.Create("Original", "Desc").Value;
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var command = new UpdateDashboardTitleCommand(dashboard.Id, "Nuevo Titulo");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        dashboard.Title.Should().Be("Nuevo Titulo");
    }

    [Fact]
    public async Task Handle_WhenDashboardNotFound_ShouldReturnFailure()
    {
        _dashboardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Dashboard?)null);

        var command = new UpdateDashboardTitleCommand(Guid.NewGuid(), "Nuevo Titulo");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.NotFound");
    }

    [Fact]
    public async Task Handle_WithEmptyTitle_ShouldReturnFailure()
    {
        var dashboard = Dashboard.Create("Original", "Desc").Value;
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var command = new UpdateDashboardTitleCommand(dashboard.Id, "");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.EmptyTitle");
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldNotCallSave()
    {
        _dashboardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Dashboard?)null);

        var command = new UpdateDashboardTitleCommand(Guid.NewGuid(), "Titulo");

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
