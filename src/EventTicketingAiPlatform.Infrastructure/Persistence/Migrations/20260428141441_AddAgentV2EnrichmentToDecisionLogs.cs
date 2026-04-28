using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentV2EnrichmentToDecisionLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "business_impact",
                table: "agent_decision_logs",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "confidence_score",
                table: "agent_decision_logs",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "enrichment_provider",
                table: "agent_decision_logs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "operator_summary",
                table: "agent_decision_logs",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "suggested_next_actions",
                table: "agent_decision_logs",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "business_impact",
                table: "agent_decision_logs");

            migrationBuilder.DropColumn(
                name: "confidence_score",
                table: "agent_decision_logs");

            migrationBuilder.DropColumn(
                name: "enrichment_provider",
                table: "agent_decision_logs");

            migrationBuilder.DropColumn(
                name: "operator_summary",
                table: "agent_decision_logs");

            migrationBuilder.DropColumn(
                name: "suggested_next_actions",
                table: "agent_decision_logs");
        }
    }
}
