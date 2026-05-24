USE [DocManagementDb]
GO

IF COL_LENGTH('X_Documents', 'Location') IS NULL
BEGIN
    ALTER TABLE [dbo].[X_Documents] ADD [Location] nvarchar(max) NULL;
END
GO

IF COL_LENGTH('X_Documents', 'IpAddress') IS NULL
BEGIN
    ALTER TABLE [dbo].[X_Documents] ADD [IpAddress] nvarchar(max) NULL;
END
GO

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
    e.Name AS EquipmentName,
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
    xd.Location,
    xd.IpAddress
FROM X_Documents xd
LEFT JOIN DocumentStatus ds ON xd.DocumentStatusId = ds.Id
LEFT JOIN Zones z ON xd.ZoneId = z.Id
LEFT JOIN Equipments e ON xd.EquipmentId = e.Id
LEFT JOIN AspNetUsers u1 ON xd.UserId = u1.Id
LEFT JOIN AspNetUsers u2 ON xd.EditedByUserId = u2.Id
LEFT JOIN DocumentTypes dt ON xd.DocumentTypeId = dt.Id;
GO

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
        i.ZoneId, i.EquipmentId,
        GETDATE(), 1, i.DocumentTypeId, ded.Location, ded.IpAddress
    FROM inserted i
    LEFT JOIN DocumentEquipmentDetails ded ON i.Id = ded.DocumentId;
END;
GO

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
        i.ZoneId, i.EquipmentId,
        GETDATE(), 2, i.DocumentTypeId, ded.Location, ded.IpAddress
    FROM inserted i
    LEFT JOIN DocumentEquipmentDetails ded ON i.Id = ded.DocumentId;
END;
GO

CREATE OR ALTER TRIGGER [dbo].[trg_DocumentEquipmentDetails_InsertUpdate]
ON [dbo].[DocumentEquipmentDetails]
AFTER INSERT, UPDATE
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
        d.Id, d.Title, d.DocumentNumber, d.CreatedAt, d.DocRegisterDate, d.Note,
        d.DocumentStatusId, d.UserId, d.EditedByUserId, d.EditedAt,
        d.ZoneId, d.EquipmentId,
        GETDATE(), 2, d.DocumentTypeId, i.Location, i.IpAddress
    FROM inserted i
    INNER JOIN Documents d ON i.DocumentId = d.Id;
END;
GO

CREATE OR ALTER TRIGGER [dbo].[trg_DocumentEquipmentDetails_Delete]
ON [dbo].[DocumentEquipmentDetails]
AFTER DELETE
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
        d.Id, d.Title, d.DocumentNumber, d.CreatedAt, d.DocRegisterDate, d.Note,
        d.DocumentStatusId, d.UserId, d.EditedByUserId, d.EditedAt,
        d.ZoneId, d.EquipmentId,
        GETDATE(), 2, d.DocumentTypeId, NULL, NULL
    FROM deleted del
    INNER JOIN Documents d ON del.DocumentId = d.Id;
END;
GO
