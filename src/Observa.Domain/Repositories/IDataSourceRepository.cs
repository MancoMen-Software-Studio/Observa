using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Observa.Domain.Entities;
using Observa.Domain.Enums;

namespace Observa.Domain.Repositories;

/// <summary>
/// Repositorio para consultar y persistir origenes de datos.
/// </summary>
public interface IDataSourceRepository
{
    Task<DataSource?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DataSource>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DataSource>> GetByTypeAsync(DataSourceType type, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DataSource>> GetActiveAsync(CancellationToken cancellationToken = default);

    Task AddAsync(DataSource dataSource, CancellationToken cancellationToken = default);

    void Update(DataSource dataSource);

    void Remove(DataSource dataSource);
}
