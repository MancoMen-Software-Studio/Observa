using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Observa.Domain.Entities;

namespace Observa.Domain.Repositories;

/// <summary>
/// Repositorio para consultar y persistir alertas del sistema.
/// </summary>
public interface IAlertRepository
{
    Task<Alert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Alert>> GetByDashboardIdAsync(Guid dashboardId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Alert>> GetUnacknowledgedAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Alert alert, CancellationToken cancellationToken = default);
}
