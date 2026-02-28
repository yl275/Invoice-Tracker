using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "user_demo");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Invoices",
                type: "text",
                nullable: false,
                defaultValue: "user_demo");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Clients",
                type: "text",
                nullable: false,
                defaultValue: "user_demo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Clients");
        }
    }
}
