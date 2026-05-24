public class DocumentAuditViewModel
{
    public int LogId { get; set; }
    public DateTime LogInsertDateTime { get; set; }

    public int DocumentId { get; set; }
    public string? Title { get; set; }
    //public string DocumentNumber { get; set; }

    public DateTime CreatedAt { get; set; }
    //public DateTime? DocRegisterDate { get; set; }

    public string? Note { get; set; }

    public int? DocumentStatusId { get; set; }
    public string StatusName { get; set; }

    public string? ZoneName { get; set; }
    public string? EquipmentName { get; set; }

    public string CreatedByUserName { get; set; }
    public string? EditedByUserName { get; set; }
    public DateTime? EditedAt { get; set; }

    public int LogActionId { get; set; }
    public string LogActionName { get; set; }

    public string? DocumentTypeName { get; set; }

    // Yeni əlavə edilən sahələr
    public string? Location { get; set; }
    public string? IpAddress { get; set; }
}
