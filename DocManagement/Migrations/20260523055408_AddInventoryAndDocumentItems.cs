using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryAndDocumentItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Documents_Equipments_EquipmentId",
            //     table: "Documents");

            // migrationBuilder.DropForeignKey(
            //     name: "FK_Documents_Zones_ZoneId",
            //     table: "Documents");

            migrationBuilder.DropTable(
                name: "DocumentEquipmentDetails");

            // migrationBuilder.DropIndex(
            //     name: "IX_Documents_EquipmentId",
            //     table: "Documents");

            // migrationBuilder.DropIndex(
            //     name: "IX_Documents_ZoneId",
            //     table: "Documents");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ZoneId",
                table: "Documents");

            migrationBuilder.CreateTable(
                name: "DocumentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    InventoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentItems_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentItems_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipmentId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CurrentLocation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CurrentIpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventories_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentItems_DocumentId",
                table: "DocumentItems",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentItems_EquipmentId",
                table: "DocumentItems",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_EquipmentId",
                table: "Inventories",
                column: "EquipmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentItems");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Equipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "Equipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ZoneId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentEquipmentDetails",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentEquipmentDetails", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_DocumentEquipmentDetails_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_EquipmentId",
                table: "Documents",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ZoneId",
                table: "Documents",
                column: "ZoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Equipments_EquipmentId",
                table: "Documents",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Zones_ZoneId",
                table: "Documents",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id");
        }
    }
}
