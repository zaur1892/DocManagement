using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditTriggersForMultipleEquipments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
                ALTER TRIGGER [dbo].[trg_Documents_Insert]
                ON [dbo].[Documents]
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    INSERT INTO X_Documents
                    (
                        Id, Title, DocumentNumber, CreatedAt, DocRegisterDate, Note,
                        DocumentStatusId, UserId, EditedByUserId, EditedAt,
                        ZoneId, EquipmentId,
                        LogInsertDateTime, LogActionId, DocumentTypeId, Location, IpAddress
                    )
                    SELECT
                        i.Id, i.Title, i.DocumentNumber, i.CreatedAt, i.DocRegisterDate, i.Note,
                        i.DocumentStatusId, i.UserId, i.EditedByUserId, i.EditedAt,
                        i.ZoneId, NULL AS EquipmentId,
                        GETDATE(), 1, i.DocumentTypeId, NULL AS Location, NULL AS IpAddress
                    FROM inserted i;
                END;
            ");

            migrationBuilder.Sql(@"
                ALTER TRIGGER [dbo].[trg_Documents_Update]
                ON [dbo].[Documents]
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    INSERT INTO X_Documents
                    (
                        Id, Title, DocumentNumber, CreatedAt, DocRegisterDate, Note,
                        DocumentStatusId, UserId, EditedByUserId, EditedAt,
                        ZoneId, EquipmentId,
                        LogInsertDateTime, LogActionId, DocumentTypeId, Location, IpAddress
                    )
                    SELECT
                        i.Id, i.Title, i.DocumentNumber, i.CreatedAt, i.DocRegisterDate, i.Note,
                        i.DocumentStatusId, i.UserId, i.EditedByUserId, i.EditedAt,
                        i.ZoneId, NULL AS EquipmentId,
                        GETDATE(), 2, i.DocumentTypeId, 
                        di.Locations AS Location, di.IpAddresses AS IpAddress
                    FROM inserted i
                    OUTER APPLY (
                        SELECT 
                            STRING_AGG(loc, ', ') AS Locations,
                            STRING_AGG(ip, ', ') AS IpAddresses
                        FROM (
                            SELECT Location AS loc, IpAddress AS ip
                            FROM DocumentItems
                            WHERE DocumentId = i.Id
                        ) t
                    ) di;
                END;
            ");

            migrationBuilder.Sql(@"
                ALTER VIEW [dbo].[vw_X_Documents_Audit]
                AS
                SELECT
                    xd.LogId,
                    xd.LogInsertDateTime,
                    xd.Id AS DocumentId,
                    xd.Title,
                    xd.DocumentNumber,
                    xd.CreatedAt,
                    xd.DocRegisterDate,
                    xd.Note,
                    xd.DocumentStatusId,
                    ds.Name AS StatusName,
                    z.Name AS ZoneName,
                    COALESCE(e.Name, di.EquipmentNames) AS EquipmentName,
                    u1.UserName AS CreatedByUserName,
                    u2.UserName AS EditedByUserName,
                    xd.EditedAt,
                    xd.LogActionId,
                    CASE xd.LogActionId
                        WHEN 1 THEN N'Əlavə edilib'
                        WHEN 2 THEN N'Dəyişdirilib'
                        ELSE N'Naməlum'
                    END AS LogActionName,
                    dt.Name AS DocumentTypeName,
                    COALESCE(xd.Location, di.Locations) AS Location,
                    COALESCE(xd.IpAddress, di.IpAddresses) AS IpAddress
                FROM X_Documents xd
                LEFT JOIN DocumentStatus ds ON xd.DocumentStatusId = ds.Id
                LEFT JOIN Zones z ON xd.ZoneId = z.Id
                LEFT JOIN Equipments e ON xd.EquipmentId = e.Id
                LEFT JOIN AspNetUsers u1 ON xd.UserId = u1.Id
                LEFT JOIN AspNetUsers u2 ON xd.EditedByUserId = u2.Id
                LEFT JOIN DocumentTypes dt ON xd.DocumentTypeId = dt.Id
                OUTER APPLY (
                    SELECT 
                        STRING_AGG(eq.Name, ', ') AS EquipmentNames,
                        STRING_AGG(i.Location, ', ') AS Locations,
                        STRING_AGG(i.IpAddress, ', ') AS IpAddresses
                    FROM DocumentItems i
                    LEFT JOIN Equipments eq ON i.EquipmentId = eq.Id
                    WHERE i.DocumentId = xd.Id
                ) di;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
