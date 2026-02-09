using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Observa.Application.Queries.Dashboards;
using Observa.Domain.Aggregates;
using Observa.Domain.Entities;
using Observa.Domain.Enums;
using Observa.Domain.Repositories;
using Observa.Domain.ValueObjects;
using Xunit;

namespace Observa.Application.Tests.Queries.Dashboards;

public sealed class GetDashboardByIdQueryHandlerTests
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly GetDashboardByIdQueryHandler _handler;

    public GetDashboardByIdQueryHandlerTests()
    {
        _dashboardRepository = Substitute.For<IDashboardRepository>();
        _handler = new GetDashboardByIdQueryHandler(_dashboardRepository);
    }

    [Fact]
    public async Task Handle_WhenDashboardExists_ShouldReturnSuccess()
    {
        var dashboard = Dashboard.Create("Mi Dashboard", "Descripcion").Value;
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var query = new GetDashboardByIdQuery(dashboard.Id);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be("Mi Dashboard");
        result.Value.Description.Should().Be("Descripcion");
        result.Value.Status.Should().Be("Draft");
    }

    [Fact]
    public async Task Handle_WhenDashboardHasWidgets_ShouldMapWidgets()
    {
        var dashboard = Dashboard.Create("Dashboard", "Desc").Value;
        var position = WidgetPosition.Create(1, 2, 4, 3).Value;
        var widget = Widget.Create("Grafico CPU", WidgetType.LineChart, position, Guid.NewGuid(), RefreshInterval.FiveSeconds).Value;
        dashboard.AddWidget(widget);
        _dashboardRepository.GetByIdAsync(dashboard.Id, Arg.Any<CancellationToken>())
            .Returns(dashboard);

        var query = new GetDashboardByIdQuery(dashboard.Id);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Widgets.Should().ContainSingle();
        var widgetResponse = result.Value.Widgets.First();
        widgetResponse.Title.Should().Be("Grafico CPU");
        widgetResponse.Column.Should().Be(1);
        widgetResponse.Row.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WhenDashboardNotFound_ShouldReturnFailure()
    {
        _dashboardRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Dashboard?)null);

        var query = new GetDashboardByIdQuery(Guid.NewGuid());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Dashboard.NotFound");
    }
}
