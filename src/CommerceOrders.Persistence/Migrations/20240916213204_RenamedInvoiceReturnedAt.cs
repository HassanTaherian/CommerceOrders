using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommerceOrders.Persistence.Migrations
{
    public partial class RenamedInvoiceReturnedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReturnDateTime",
                table: "Invoices",
                newName: "ReturnedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReturnedAt",
                table: "Invoices",
                newName: "ReturnDateTime");
        }
    }
}
