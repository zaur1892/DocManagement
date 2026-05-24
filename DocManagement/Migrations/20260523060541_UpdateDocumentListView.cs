using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDocumentListView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

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

                    CAST(NULL AS INT) AS ZoneId,
                    CAST(NULL AS INT) AS EquipmentId,
                    CAST(NULL AS NVARCHAR(255)) AS ZoneName,

                    di.EquipmentNames AS EquipmentName,
                    di.Serias AS Seria,
                    di.Models AS Model,
                    di.Locations AS Location,
                    di.IpAddresses AS IpAddress,

                    CAST(NULL AS NVARCHAR(MAX)) AS CustomFieldName1,
                    'Peh2' AS CustomFieldName2,
                    'Peh3' AS CustomFieldName3

                FROM Documents d
                LEFT JOIN DocumentTypes dt WITH(NOLOCK) ON d.DocumentTypeId = dt.Id
                INNER JOIN DocumentStatus ds WITH(NOLOCK) ON d.DocumentStatusId = ds.Id
                LEFT JOIN StatusColors sc WITH(NOLOCK) ON ds.Id = sc.DocumentStatusId
                OUTER APPLY (
                    SELECT 
                        STRING_AGG(e.Name, ', ') AS EquipmentNames,
                        STRING_AGG(i.SerialNumber, ', ') AS Serias,
                        STRING_AGG(i.Model, ', ') AS Models,
                        STRING_AGG(i.Location, ', ') AS Locations,
                        STRING_AGG(i.IpAddress, ', ') AS IpAddresses
                    FROM DocumentItems i
                    LEFT JOIN Equipments e ON i.EquipmentId = e.Id
                    WHERE i.DocumentId = d.Id
                ) di
                WHERE d.DocumentStatusId !=0
                AND d.CreatedAt > '20260101'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
