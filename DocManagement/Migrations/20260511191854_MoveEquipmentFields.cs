using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocManagement.Migrations
{
    /// <inheritdoc />
    public partial class MoveEquipmentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Model",
                table: "DocumentEquipmentDetails");

            migrationBuilder.DropColumn(
                name: "Seria",
                table: "DocumentEquipmentDetails");

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
    
                    e.SerialNumber AS Seria,
                    e.Model,
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
            migrationBuilder.DropColumn(
                name: "Model",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "Equipments");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "DocumentEquipmentDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Seria",
                table: "DocumentEquipmentDetails",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

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
    }
}
