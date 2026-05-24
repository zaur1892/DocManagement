using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentEquipmentDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.CreateTable(
                name: "DocumentEquipmentDetails",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    Seria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
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


            migrationBuilder.Sql(@"
                ALTER VIEW [dbo].[vw_DocumentList] 
                AS SELECT 
                    d.Id,
                    d.CreatedAt,
                    d.DocumentStatusId,
                    d.DocumentTypeId,
                    d.UserId,
                    d.Note,

                    dt.Name AS DocumentTypeName,
                    ds.Code AS StatusCode,
                    ds.Name AS StatusName,
                    sc.Color AS StatusColor,

	                d.ZoneId AS ZoneId,
                    d.EquipmentId AS EquipmentId,
                    z.Name AS ZoneName,
                    e.Name AS EquipmentName,
    
                    ded.Seria,
                    ded.Model,
                    ded.Location,
                    ded.IpAddress,

	                e.description CustomFieldName1,
	                'Peh2' CustomFieldName2,
	                'Peh3' CustomFieldName3

                FROM Documents d
                LEFT JOIN DocumentTypes dt WITH(NOLOCK) ON d.DocumentTypeId = dt.Id
                INNER JOIN DocumentStatus ds WITH(NOLOCK) ON d.DocumentStatusId = ds.Id
                LEFT JOIN StatusColors sc WITH(NOLOCK) ON ds.Id = sc.DocumentStatusId
                LEFT JOIN Zones z WITH(NOLOCK) ON d.ZoneId = z.Id
                LEFT JOIN Equipments e WITH(NOLOCK) ON d.EquipmentId = e.Id
                LEFT JOIN DocumentEquipmentDetails ded WITH(NOLOCK) ON d.Id = ded.DocumentId

                WHERE D.DocumentStatusId !=0
                AND D.CreatedAt > '20260101'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DocumentTypes_DocumentTypeId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "DocElements");

            migrationBuilder.DropTable(
                name: "DocumentEquipmentDetails");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentTypeId",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Documents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DocumentTypes_DocumentTypeId",
                table: "Documents",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
                ALTER VIEW [dbo].[vw_DocumentList] 
                AS SELECT 
                    d.Id,
                    d.CreatedAt,
                    d.DocumentStatusId,
                    d.DocumentTypeId,
                    d.UserId,
                    d.Note,

                    dt.Name AS DocumentTypeName,
                    ds.Code AS StatusCode,
                    ds.Name AS StatusName,
                    sc.Color AS StatusColor,

	                d.ZoneId AS ZoneId,
                    d.EquipmentId AS EquipmentId,
                    z.Name AS ZoneName,
                    e.Name AS EquipmentName,
	                e.description CustomFieldName1,
	                'Peh2' CustomFieldName2,
	                'Peh3' CustomFieldName3

                FROM Documents d
                LEFT JOIN DocumentTypes dt WITH(NOLOCK) ON d.DocumentTypeId = dt.Id
                INNER JOIN DocumentStatus ds WITH(NOLOCK) ON d.DocumentStatusId = ds.Id
                LEFT JOIN StatusColors sc WITH(NOLOCK) ON ds.Id = sc.DocumentStatusId
                LEFT JOIN Zones z WITH(NOLOCK) ON d.ZoneId = z.Id
                LEFT JOIN Equipments e WITH(NOLOCK) ON d.EquipmentId = e.Id
                WHERE D.DocumentStatusId !=0
                AND D.CreatedAt > '20260101'
            ");
        }
    }
}
