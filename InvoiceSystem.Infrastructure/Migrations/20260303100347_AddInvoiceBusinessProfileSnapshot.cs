using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceBusinessProfileSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessAbnSnapshot",
                table: "Invoices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BusinessBankAccountNumberSnapshot",
                table: "Invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessBankBsbSnapshot",
                table: "Invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessEmailSnapshot",
                table: "Invoices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BusinessNameSnapshot",
                table: "Invoices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BusinessPayIdSnapshot",
                table: "Invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessPaymentMethodSnapshot",
                table: "Invoices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BusinessPhoneSnapshot",
                table: "Invoices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BusinessPostalLocationSnapshot",
                table: "Invoices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BusinessWebsiteSnapshot",
                table: "Invoices",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessAbnSnapshot",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BusinessBankAccountNumberSnapshot",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BusinessBankBsbSnapshot",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BusinessEmailSnapshot",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BusinessNameSnapshot",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BusinessPayIdSnapshot",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BusinessPaymentMethodSnapshot",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BusinessPhoneSnapshot",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BusinessPostalLocationSnapshot",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BusinessWebsiteSnapshot",
                table: "Invoices");
        }
    }
}
