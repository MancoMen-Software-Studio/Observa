using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Observa.Domain.Entities;
using Observa.Domain.Enums;

namespace Observa.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuracion de EF Core para la entidad Alert.
/// </summary>
public sealed class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable("alerts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(a => a.DashboardId)
            .HasColumnName("dashboard_id")
            .IsRequired();

        builder.Property(a => a.Message)
            .HasColumnName("message")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.IsAcknowledged)
            .HasColumnName("is_acknowledged")
            .IsRequired();

        builder.Property(a => a.TriggeredAt)
            .HasColumnName("triggered_at")
            .IsRequired();

        builder.Property(a => a.AcknowledgedAt)
            .HasColumnName("acknowledged_at");

        builder.OwnsOne(a => a.Rule, rule =>
        {
            rule.Property(r => r.MetricName)
                .HasColumnName("rule_metric_name")
                .HasMaxLength(200)
                .IsRequired();

            rule.Property(r => r.Value)
                .HasColumnName("rule_value")
                .IsRequired();

            rule.Property(r => r.Operator)
                .HasColumnName("rule_operator")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            rule.Property(r => r.Severity)
                .HasColumnName("rule_severity")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.HasIndex(a => a.DashboardId)
            .HasDatabaseName("ix_alerts_dashboard_id");

        builder.HasIndex(a => a.IsAcknowledged)
            .HasDatabaseName("ix_alerts_is_acknowledged");
    }
}
