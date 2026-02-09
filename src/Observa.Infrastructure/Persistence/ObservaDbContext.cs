using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Observa.Domain.Abstractions;
using Observa.Domain.Aggregates;
using Observa.Domain.Entities;

namespace Observa.Infrastructure.Persistence;

/// <summary>
/// Contexto de base de datos principal de la aplicacion Observa.
/// </summary>
public sealed class ObservaDbContext : DbContext, IUnitOfWork
{
    public ObservaDbContext(DbContextOptions<ObservaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Dashboard> Dashboards => Set<Dashboard>();

    public DbSet<DataSource> DataSources => Set<DataSource>();

    public DbSet<Alert> Alerts => Set<Alert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ObservaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
