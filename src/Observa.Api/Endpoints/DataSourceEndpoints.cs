using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Observa.Application.Commands.DataSources;
using Observa.Application.Queries.DataSources;

namespace Observa.Api.Endpoints;

/// <summary>
/// Endpoints de Minimal API para operaciones con origenes de datos.
/// </summary>
public static class DataSourceEndpoints
{
    public static void MapDataSourceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/datasources")
            .WithTags("DataSources");

        group.MapGet("/", GetAllDataSources);
        group.MapPost("/", CreateDataSource);
    }

    private static async Task<IResult> GetAllDataSources(ISender sender, CancellationToken cancellationToken)
    {
        var query = new GetAllDataSourcesQuery();
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.Problem(result.Error.Description, statusCode: 400);
    }

    private static async Task<IResult> CreateDataSource(
        CreateDataSourceCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/datasources/{result.Value}", new { Id = result.Value })
            : Results.BadRequest(new { result.Error.Code, result.Error.Description });
    }
}
