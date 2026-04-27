using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventTicketingAiPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgentNotificationNoPascalCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "agent_notifications",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Severity",
                table: "agent_notifications",
                newName: "severity");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "agent_notifications",
                newName: "message");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "agent_notifications",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ScanAttemptId",
                table: "agent_notifications",
                newName: "scan_attempt_id");

            migrationBuilder.RenameColumn(
                name: "ReadAtUtc",
                table: "agent_notifications",
                newName: "read_at_utc");

            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "agent_notifications",
                newName: "is_read");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "agent_notifications",
                newName: "created_at_utc");

            migrationBuilder.RenameIndex(
                name: "IX_agent_notifications_IsRead",
                table: "agent_notifications",
                newName: "IX_agent_notifications_is_read");

            migrationBuilder.RenameIndex(
                name: "IX_agent_notifications_CreatedAtUtc",
                table: "agent_notifications",
                newName: "IX_agent_notifications_created_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_agent_notifications_scan_attempt_id",
                table: "agent_notifications",
                column: "scan_attempt_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_agent_notifications_scan_attempt_id",
                table: "agent_notifications");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "agent_notifications",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "severity",
                table: "agent_notifications",
                newName: "Severity");

            migrationBuilder.RenameColumn(
                name: "message",
                table: "agent_notifications",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "agent_notifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "scan_attempt_id",
                table: "agent_notifications",
                newName: "ScanAttemptId");

            migrationBuilder.RenameColumn(
                name: "read_at_utc",
                table: "agent_notifications",
                newName: "ReadAtUtc");

            migrationBuilder.RenameColumn(
                name: "is_read",
                table: "agent_notifications",
                newName: "IsRead");

            migrationBuilder.RenameColumn(
                name: "created_at_utc",
                table: "agent_notifications",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_agent_notifications_is_read",
                table: "agent_notifications",
                newName: "IX_agent_notifications_IsRead");

            migrationBuilder.RenameIndex(
                name: "IX_agent_notifications_created_at_utc",
                table: "agent_notifications",
                newName: "IX_agent_notifications_CreatedAtUtc");
        }
    }
}
