using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Observa.Application.Queries.Dashboards;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Enums;
using Observa.Domain.Repositories;
using Xunit;

namespace Observa.Application.Tests.Queries.Dashboards;

public sealed class GetAllDashboardsQueryHandlerTests
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly GetAllDashboardsQueryHandler _handler;

    public GetAllDashboardsQueryHandlerTests()
    {
        _dashboardRepository = Substitute.For<IDashboardRepository>();
        _handler = new GetAllDashboardsQueryHandler(_dashboardRepository);
    }

    [Fact]
    public async Task Handle_WhenDashboardsExist_ShouldReturnPaged()
    {
        var dashboards = new List<Dashboard>
        {
            Dashboard.Create("Dashboard 1", "Desc 1").Value,
            Dashboard.Create("Dashboard 2", "Desc 2").Value,
            Dashboard.Create("Dashboard 3", "Desc 3").Value
        };
        var pagedResult = new PagedResult<Dashboard>(dashboards.AsReadOnly(), 3, 1, 20);
        _dashboardRepository.GetPagedAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<DashboardStatus?>(),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var query = new GetAllDashboardsQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(3);
        result.Value.TotalCount.Should().Be(3);
        result.Value.Page.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WhenNoDashboards_ShouldReturnEmptyPage()
    {
        var pagedResult = new PagedResult<Dashboard>(new List<Dashboard>().AsReadOnly(), 0, 1, 20);
        _dashboardRepository.GetPagedAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<DashboardStatus?>(),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var query = new GetAllDashboardsQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldMapCorrectFields()
    {
        var dashboard = Dashboard.Create("Test Dashboard", "Test Desc").Value;
        var pagedResult = new PagedResult<Dashboard>(
            new List<Dashboard> { dashboard }.AsReadOnly(), 1, 1, 20);
        _dashboardRepository.GetPagedAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<DashboardStatus?>(),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var query = new GetAllDashboardsQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var response = result.Value.Items.Should().ContainSingle().Which;
        response.Title.Should().Be("Test Dashboard");
        response.Description.Should().Be("Test Desc");
        response.Status.Should().Be("Draft");
    }
}
