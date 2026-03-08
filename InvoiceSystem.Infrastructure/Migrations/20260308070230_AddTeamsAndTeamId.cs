using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamsAndTeamId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    InvitedByUserId = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamInvitations_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => new { x.TeamId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TeamMembers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamInvitations_TeamId",
                table: "TeamInvitations",
                column: "TeamId");

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "Clients",
                type: "uuid",
                nullable: true);
            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "Products",
                type: "uuid",
                nullable: true);
            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "Invoices",
                type: "uuid",
                nullable: true);

            // Backfill: for each distinct UserId create a personal team and set TeamId on existing rows
            migrationBuilder.Sql(@"
                DO $$
                DECLARE r RECORD; tid uuid;
                BEGIN
                    FOR r IN (SELECT DISTINCT ""UserId"" FROM ""Clients"" UNION SELECT DISTINCT ""UserId"" FROM ""Products"" UNION SELECT DISTINCT ""UserId"" FROM ""Invoices"")
                    LOOP
                        tid := gen_random_uuid();
                        INSERT INTO ""Teams"" (""Id"", ""Name"", ""CreatedAt"") VALUES (tid, 'My workspace', NOW());
                        INSERT INTO ""TeamMembers"" (""TeamId"", ""UserId"", ""Role"", ""JoinedAt"") VALUES (tid, r.""UserId"", 0, NOW());
                        UPDATE ""Clients"" SET ""TeamId"" = tid WHERE ""UserId"" = r.""UserId"";
                        UPDATE ""Products"" SET ""TeamId"" = tid WHERE ""UserId"" = r.""UserId"";
                        UPDATE ""Invoices"" SET ""TeamId"" = tid WHERE ""UserId"" = r.""UserId"";
                    END LOOP;
                END $$;
            ");
            migrationBuilder.Sql(@"
                ALTER TABLE ""Clients"" ALTER COLUMN ""TeamId"" SET NOT NULL;
                ALTER TABLE ""Products"" ALTER COLUMN ""TeamId"" SET NOT NULL;
                ALTER TABLE ""Invoices"" ALTER COLUMN ""TeamId"" SET NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamInvitations");

            migrationBuilder.DropTable(
                name: "TeamMembers");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Clients");
        }
    }
}
