using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Observa.Application.Commands.Dashboards;
using Observa.Application.Commands.Widgets;
using Observa.Application.DTOs;
using Observa.Application.Queries.Dashboards;

namespace Observa.Api.Endpoints;

/// <summary>
/// Endpoints de Minimal API para operaciones con dashboards.
/// </summary>
public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dashboards")
            .WithTags("Dashboards");

        group.MapGet("/", GetAll);
        group.MapGet("/{id:guid}", GetById);
        group.MapPost("/", Create);
        group.MapPut("/{id:guid}/title", UpdateTitle);
        group.MapPost("/{id:guid}/publish", Publish);
        group.MapPost("/{id:guid}/archive", Archive);
        group.MapPost("/{id:guid}/widgets", AddWidget);
        group.MapDelete("/{id:guid}/widgets/{widgetId:guid}", RemoveWidget);
    }

    private static async Task<IResult> GetAll(
        ISender sender,
        CancellationToken cancellationToken,
        int page = 1,
        int pageSize = 20,
        string? status = null,
        string? search = null)
    {
        var query = new GetAllDashboardsQuery(page, pageSize, status, search);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(result.Error.Description, statusCode: 400);
    }

    private static async Task<IResult> GetById(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var query = new GetDashboardByIdQuery(id);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new { result.Error.Code, result.Error.Description });
    }

    private static async Task<IResult> Create(
        CreateDashboardCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/dashboards/{result.Value}", new { Id = result.Value })
            : Results.BadRequest(new { result.Error.Code, result.Error.Description });
    }

    private static async Task<IResult> UpdateTitle(
        Guid id,
        UpdateTitleRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDashboardTitleCommand(id, request.NewTitle);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new { result.Error.Code, result.Error.Description });
    }

    private static async Task<IResult> Publish(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var command = new PublishDashboardCommand(id);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new { result.Error.Code, result.Error.Description });
    }

    private static async Task<IResult> Archive(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var command = new ArchiveDashboardCommand(id);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(new { result.Error.Code, result.Error.Description });
    }

    private static async Task<IResult> AddWidget(
        Guid id,
        AddWidgetCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var fullCommand = command with { DashboardId = id };
        var result = await sender.Send(fullCommand, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/dashboards/{id}/widgets/{result.Value}", new { Id = result.Value })
            : Results.BadRequest(new { result.Error.Code, result.Error.Description });
    }

    private static async Task<IResult> RemoveWidget(
        Guid id,
        Guid widgetId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RemoveWidgetCommand(id, widgetId);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(new { result.Error.Code, result.Error.Description });
    }
}

/// <summary>
/// Request para actualizar el titulo de un dashboard.
/// </summary>
public sealed record UpdateTitleRequest(string NewTitle);
