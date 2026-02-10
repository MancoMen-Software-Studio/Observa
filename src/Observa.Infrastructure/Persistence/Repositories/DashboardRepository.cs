using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Enums;
using Observa.Domain.Repositories;

namespace Observa.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementacion del repositorio de dashboards con EF Core.
/// </summary>
public sealed class DashboardRepository : IDashboardRepository
{
    private readonly ObservaDbContext _dbContext;

    public DashboardRepository(ObservaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dashboard?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Dashboards
            .Include(d => d.Widgets)
            .Include(d => d.ThresholdRules)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Dashboard>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var dashboards = await _dbContext.Dashboards
            .Include(d => d.Widgets)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return dashboards.AsReadOnly();
    }

    /// <summary>
    /// Obtiene dashboards paginados con filtros opcionales de estado y busqueda.
    /// </summary>
    public async Task<PagedResult<Dashboard>> GetPagedAsync(
        int page,
        int pageSize,
        DashboardStatus? statusFilter = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Dashboards
            .Include(d => d.Widgets)
            .AsNoTracking()
            .AsQueryable();

        if (statusFilter.HasValue)
        {
            query = query.Where(d => d.Status == statusFilter.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(d => EF.Functions.ILike(d.Title, $"%{searchTerm}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Dashboard>(items.AsReadOnly(), totalCount, page, pageSize);
    }

    public async Task<IReadOnlyCollection<Dashboard>> GetByStatusAsync(
        DashboardStatus status,
        CancellationToken cancellationToken = default)
    {
        var dashboards = await _dbContext.Dashboards
            .Include(d => d.Widgets)
            .Where(d => d.Status == status)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return dashboards.AsReadOnly();
    }

    public async Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Dashboards
            .AnyAsync(d => d.Title == title, cancellationToken);
    }

    public async Task AddAsync(Dashboard entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Dashboards.AddAsync(entity, cancellationToken);
    }

    public void Update(Dashboard entity)
    {
        _dbContext.Dashboards.Update(entity);
    }

    public void Remove(Dashboard entity)
    {
        _dbContext.Dashboards.Remove(entity);
    }
}
