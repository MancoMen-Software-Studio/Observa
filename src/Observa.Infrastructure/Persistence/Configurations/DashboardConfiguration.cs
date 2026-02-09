using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Observa.Domain.Aggregates;
using Observa.Domain.Enums;

namespace Observa.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuracion de EF Core para el agregado Dashboard.
/// </summary>
public sealed class DashboardConfiguration : IEntityTypeConfiguration<Dashboard>
{
    public void Configure(EntityTypeBuilder<Dashboard> builder)
    {
        builder.ToTable("dashboards");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(d => d.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(d => d.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        builder.Property(d => d.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasMany(d => d.Widgets)
            .WithOne()
            .HasForeignKey("dashboard_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsMany(d => d.ThresholdRules, rule =>
        {
            rule.ToTable("dashboard_threshold_rules");

            rule.Property(r => r.MetricName)
                .HasColumnName("metric_name")
                .HasMaxLength(200)
                .IsRequired();

            rule.Property(r => r.Value)
                .HasColumnName("value")
                .IsRequired();

            rule.Property(r => r.Operator)
                .HasColumnName("operator")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            rule.Property(r => r.Severity)
                .HasColumnName("severity")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.Ignore(d => d.DomainEvents);
    }
}
