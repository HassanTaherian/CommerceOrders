using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommerceOrders.Persistence.Migrations
{
    public partial class CorrectNameOfIsReturnned : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsReturn",
                table: "InvoiceItems",
                newName: "IsReturned");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsReturned",
                table: "InvoiceItems",
                newName: "IsReturn");
        }
    }
}
