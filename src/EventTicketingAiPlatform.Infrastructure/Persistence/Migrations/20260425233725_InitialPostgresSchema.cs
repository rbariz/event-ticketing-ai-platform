using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgresSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    starts_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ends_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "scan_attempts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ticket_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ticket_id = table.Column<Guid>(type: "uuid", nullable: true),
                    device_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    gate_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    scanned_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    decision = table.Column<int>(type: "integer", nullable: false),
                    reason_code = table.Column<int>(type: "integer", nullable: false),
                    source_ip = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    processing_time_ms = table.Column<long>(type: "bigint", nullable: false),
                    correlation_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scan_attempts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tickets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ticket_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    valid_from_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    valid_until_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    consumed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tickets", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_scan_attempts_gate_id",
                table: "scan_attempts",
                column: "gate_id");

            migrationBuilder.CreateIndex(
                name: "IX_scan_attempts_scanned_at_utc",
                table: "scan_attempts",
                column: "scanned_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_scan_attempts_source",
                table: "scan_attempts",
                column: "source");

            migrationBuilder.CreateIndex(
                name: "IX_scan_attempts_ticket_code",
                table: "scan_attempts",
                column: "ticket_code");

            migrationBuilder.CreateIndex(
                name: "IX_scan_attempts_ticket_code_scanned_at_utc",
                table: "scan_attempts",
                columns: new[] { "ticket_code", "scanned_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_tickets_event_id",
                table: "tickets",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_ticket_code",
                table: "tickets",
                column: "ticket_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "scan_attempts");

            migrationBuilder.DropTable(
                name: "tickets");
        }
    }
}
