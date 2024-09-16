using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommerceOrders.Persistence.Migrations
{
    public partial class RenamedOriginalPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "InvoiceItems",
                newName: "OriginalPrice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginalPrice",
                table: "InvoiceItems",
                newName: "Price");
        }
    }
}
