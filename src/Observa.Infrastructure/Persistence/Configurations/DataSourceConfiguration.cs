using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Observa.Domain.Entities;
using Observa.Domain.Enums;

namespace Observa.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuracion de EF Core para la entidad DataSource.
/// </summary>
public sealed class DataSourceConfiguration : IEntityTypeConfiguration<DataSource>
{
    public void Configure(EntityTypeBuilder<DataSource> builder)
    {
        builder.ToTable("data_sources");

        builder.HasKey(ds => ds.Id);

        builder.Property(ds => ds.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(ds => ds.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ds => ds.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ds => ds.ConnectionString)
            .HasColumnName("connection_string")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(ds => ds.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(ds => ds.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(ds => ds.LastSyncAt)
            .HasColumnName("last_sync_at");
    }
}
