using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Observa.Domain.Entities;
using Observa.Domain.Repositories;

namespace Observa.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementacion del repositorio de alertas con EF Core.
/// </summary>
public sealed class AlertRepository : IAlertRepository
{
    private readonly ObservaDbContext _dbContext;

    public AlertRepository(ObservaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Alert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Alerts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Alert>> GetByDashboardIdAsync(
        Guid dashboardId,
        CancellationToken cancellationToken = default)
    {
        var alerts = await _dbContext.Alerts
            .Where(a => a.DashboardId == dashboardId)
            .OrderByDescending(a => a.TriggeredAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return alerts.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Alert>> GetUnacknowledgedAsync(
        CancellationToken cancellationToken = default)
    {
        var alerts = await _dbContext.Alerts
            .Where(a => !a.IsAcknowledged)
            .OrderByDescending(a => a.TriggeredAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return alerts.AsReadOnly();
    }

    public async Task AddAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        await _dbContext.Alerts.AddAsync(alert, cancellationToken);
    }
}
