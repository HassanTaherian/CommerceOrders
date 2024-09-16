using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommerceOrders.Persistence.Migrations
{
    public partial class RenamedFinalPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NewPrice",
                table: "InvoiceItems",
                newName: "FinalPrice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FinalPrice",
                table: "InvoiceItems",
                newName: "NewPrice");
        }
    }
}
