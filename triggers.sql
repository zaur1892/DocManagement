USE [DocManagementDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
        LogInsertDateTime, LogActionId,DocumentTypeId
    )
    SELECT
        i.Id, i.Title, i.DocumentNumber, i.CreatedAt, i.DocRegisterDate, i.Note,
        i.DocumentStatusId, i.UserId, i.EditedByUserId, i.EditedAt,
        i.ZoneId, i.EquipmentId,
        GETDATE(), 1,DocumentTypeId
    FROM inserted i;
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
        LogInsertDateTime, LogActionId,DocumentTypeId
    )
    SELECT
        i.Id, i.Title, i.DocumentNumber, i.CreatedAt, i.DocRegisterDate, i.Note,
        i.DocumentStatusId, i.UserId, i.EditedByUserId, i.EditedAt,
        i.ZoneId, i.EquipmentId,
        GETDATE(), 2,DocumentTypeId
    FROM inserted i;
END;
GO
