using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Observa.Application.Abstractions.Messaging;
using Observa.Application.DTOs;
using Observa.Domain.Abstractions;
using Observa.Domain.Repositories;

namespace Observa.Application.Queries.DataSources;

/// <summary>
/// Consulta para obtener todos los origenes de datos del sistema.
/// </summary>
public sealed record GetAllDataSourcesQuery : IQuery<IReadOnlyCollection<DataSourceResponse>>;

/// <summary>
/// Handler que procesa la consulta de todos los origenes de datos.
/// </summary>
public sealed class GetAllDataSourcesQueryHandler
    : IQueryHandler<GetAllDataSourcesQuery, IReadOnlyCollection<DataSourceResponse>>
{
    private readonly IDataSourceRepository _dataSourceRepository;

    public GetAllDataSourcesQueryHandler(IDataSourceRepository dataSourceRepository)
    {
        _dataSourceRepository = dataSourceRepository;
    }

    public async Task<Result<IReadOnlyCollection<DataSourceResponse>>> Handle(
        GetAllDataSourcesQuery request,
        CancellationToken cancellationToken)
    {
        var dataSources = await _dataSourceRepository.GetAllAsync(cancellationToken);

        var response = dataSources
            .Select(ds => new DataSourceResponse(
                ds.Id,
                ds.Name,
                ds.Type.ToString(),
                ds.IsActive,
                ds.CreatedAt,
                ds.LastSyncAt))
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyCollection<DataSourceResponse>>.Success(response);
    }
}
