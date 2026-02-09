using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Observa.Domain.Entities;
using Observa.Domain.Enums;

namespace Observa.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuracion de EF Core para la entidad Widget.
/// </summary>
public sealed class WidgetConfiguration : IEntityTypeConfiguration<Widget>
{
    public void Configure(EntityTypeBuilder<Widget> builder)
    {
        builder.ToTable("widgets");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(w => w.Title)
            .HasColumnName("title")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(w => w.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(w => w.DataSourceId)
            .HasColumnName("data_source_id")
            .IsRequired();

        builder.Property(w => w.RefreshInterval)
            .HasColumnName("refresh_interval")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(w => w.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(w => w.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.OwnsOne(w => w.Position, position =>
        {
            position.Property(p => p.Column)
                .HasColumnName("position_column")
                .IsRequired();

            position.Property(p => p.Row)
                .HasColumnName("position_row")
                .IsRequired();

            position.Property(p => p.Width)
                .HasColumnName("position_width")
                .IsRequired();

            position.Property(p => p.Height)
                .HasColumnName("position_height")
                .IsRequired();
        });
    }
}
