using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Observa.Application.Commands.DataSources;

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

        group.MapPost("/", Create);
    }

    private static async Task<IResult> Create(
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
