using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentDecisionLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "agent_decision_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    scan_attempt_id = table.Column<Guid>(type: "uuid", nullable: false),
                    risk_score = table.Column<int>(type: "integer", nullable: false),
                    risk_level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    severity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    actions = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    requires_human_review = table.Column<bool>(type: "boolean", nullable: false),
                    provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agent_decision_logs", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_agent_decision_logs_created_at_utc",
                table: "agent_decision_logs",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_agent_decision_logs_scan_attempt_id",
                table: "agent_decision_logs",
                column: "scan_attempt_id");

            migrationBuilder.CreateIndex(
                name: "IX_agent_decision_logs_severity",
                table: "agent_decision_logs",
                column: "severity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agent_decision_logs");
        }
    }
}
