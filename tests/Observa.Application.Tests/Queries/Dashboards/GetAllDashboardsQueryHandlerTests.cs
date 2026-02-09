using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Observa.Application.Queries.Dashboards;
using Observa.Domain.Aggregates;
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
    public async Task Handle_WhenDashboardsExist_ShouldReturnAll()
    {
        var dashboards = new List<Dashboard>
        {
            Dashboard.Create("Dashboard 1", "Desc 1").Value,
            Dashboard.Create("Dashboard 2", "Desc 2").Value,
            Dashboard.Create("Dashboard 3", "Desc 3").Value
        };
        _dashboardRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(dashboards.AsReadOnly());

        var query = new GetAllDashboardsQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WhenNoDashboards_ShouldReturnEmptyCollection()
    {
        _dashboardRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Dashboard>().AsReadOnly());

        var query = new GetAllDashboardsQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldMapCorrectFields()
    {
        var dashboard = Dashboard.Create("Test Dashboard", "Test Desc").Value;
        _dashboardRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Dashboard> { dashboard }.AsReadOnly());

        var query = new GetAllDashboardsQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var response = result.Value.Should().ContainSingle().Which;
        response.Title.Should().Be("Test Dashboard");
        response.Description.Should().Be("Test Desc");
        response.Status.Should().Be("Draft");
    }
}
