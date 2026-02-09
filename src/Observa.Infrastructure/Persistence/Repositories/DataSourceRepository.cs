using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Observa.Domain.Entities;
using Observa.Domain.Enums;
using Observa.Domain.Repositories;

namespace Observa.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementacion del repositorio de origenes de datos con EF Core.
/// </summary>
public sealed class DataSourceRepository : IDataSourceRepository
{
    private readonly ObservaDbContext _dbContext;

    public DataSourceRepository(ObservaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DataSource?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DataSources
            .FirstOrDefaultAsync(ds => ds.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<DataSource>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var dataSources = await _dbContext.DataSources
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return dataSources.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<DataSource>> GetByTypeAsync(
        DataSourceType type,
        CancellationToken cancellationToken = default)
    {
        var dataSources = await _dbContext.DataSources
            .Where(ds => ds.Type == type)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return dataSources.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<DataSource>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var dataSources = await _dbContext.DataSources
            .Where(ds => ds.IsActive)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return dataSources.AsReadOnly();
    }

    public async Task AddAsync(DataSource dataSource, CancellationToken cancellationToken = default)
    {
        await _dbContext.DataSources.AddAsync(dataSource, cancellationToken);
    }

    public void Update(DataSource dataSource)
    {
        _dbContext.DataSources.Update(dataSource);
    }

    public void Remove(DataSource dataSource)
    {
        _dbContext.DataSources.Remove(dataSource);
    }
}
