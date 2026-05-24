using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryTypeAndRemoveInventoryAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InventoryAction",
                table: "DocumentTypes");

            migrationBuilder.AddColumn<int>(
                name: "InventoryTypeId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InventoryTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_InventoryTypeId",
                table: "Documents",
                column: "InventoryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_InventoryTypes_InventoryTypeId",
                table: "Documents",
                column: "InventoryTypeId",
                principalTable: "InventoryTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_InventoryTypes_InventoryTypeId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "InventoryTypes");

            migrationBuilder.DropIndex(
                name: "IX_Documents_InventoryTypeId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "InventoryTypeId",
                table: "Documents");

            migrationBuilder.AddColumn<int>(
                name: "InventoryAction",
                table: "DocumentTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
