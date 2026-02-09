using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Observa.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "alerts",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                dashboard_id = table.Column<Guid>(type: "uuid", nullable: false),
                rule_metric_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                rule_value = table.Column<double>(type: "double precision", nullable: false),
                rule_operator = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                rule_severity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                is_acknowledged = table.Column<bool>(type: "boolean", nullable: false),
                triggered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                acknowledged_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_alerts", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "dashboards",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_dashboards", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "data_sources",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                connection_string = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                last_sync_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_data_sources", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "dashboard_threshold_rules",
            columns: table => new
            {
                DashboardId = table.Column<Guid>(type: "uuid", nullable: false),
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                metric_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                value = table.Column<double>(type: "double precision", nullable: false),
                @operator = table.Column<string>(name: "operator", type: "character varying(50)", maxLength: 50, nullable: false),
                severity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_dashboard_threshold_rules", x => new { x.DashboardId, x.Id });
                table.ForeignKey(
                    name: "FK_dashboard_threshold_rules_dashboards_DashboardId",
                    column: x => x.DashboardId,
                    principalTable: "dashboards",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "widgets",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                position_column = table.Column<int>(type: "integer", nullable: false),
                position_row = table.Column<int>(type: "integer", nullable: false),
                position_width = table.Column<int>(type: "integer", nullable: false),
                position_height = table.Column<int>(type: "integer", nullable: false),
                data_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                refresh_interval = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                dashboard_id = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_widgets", x => x.id);
                table.ForeignKey(
                    name: "FK_widgets_dashboards_dashboard_id",
                    column: x => x.dashboard_id,
                    principalTable: "dashboards",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_alerts_dashboard_id",
            table: "alerts",
            column: "dashboard_id");

        migrationBuilder.CreateIndex(
            name: "ix_alerts_is_acknowledged",
            table: "alerts",
            column: "is_acknowledged");

        migrationBuilder.CreateIndex(
            name: "IX_widgets_dashboard_id",
            table: "widgets",
            column: "dashboard_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "alerts");

        migrationBuilder.DropTable(
            name: "dashboard_threshold_rules");

        migrationBuilder.DropTable(
            name: "data_sources");

        migrationBuilder.DropTable(
            name: "widgets");

        migrationBuilder.DropTable(
            name: "dashboards");
    }
}
