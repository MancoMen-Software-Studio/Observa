using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Enums;

namespace Observa.Domain.Repositories;

/// <summary>
/// Repositorio especializado para el agregado Dashboard.
/// </summary>
public interface IDashboardRepository : IRepository<Dashboard>
{
    Task<IReadOnlyCollection<Dashboard>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Dashboard>> GetByStatusAsync(DashboardStatus status, CancellationToken cancellationToken = default);

    Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default);
}
