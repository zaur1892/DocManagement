using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddQtyToDocumentItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_InventoryTypes_InventoryTypeId",
                table: "Documents");

            migrationBuilder.AddColumn<int>(
                name: "Qty",
                table: "DocumentItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_InventoryTypes_InventoryTypeId",
                table: "Documents",
                column: "InventoryTypeId",
                principalTable: "InventoryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_InventoryTypes_InventoryTypeId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Qty",
                table: "DocumentItems");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_InventoryTypes_InventoryTypeId",
                table: "Documents",
                column: "InventoryTypeId",
                principalTable: "InventoryTypes",
                principalColumn: "Id");
        }
    }
}
