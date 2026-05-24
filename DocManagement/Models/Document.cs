using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocManagement.Models
{
    public class Document
    {
        public int Id { get; set; }

        public int? DocumentTypeId { get; set; }
        [ForeignKey("DocumentTypeId")]
        public DocumentType? DocumentType { get; set; }

        // İnventar Əməliyyat Tipi (Alış, Təmir, Yerdəyişmə...)
        [Required(ErrorMessage = "İnventara Təsir Növü (Tip) seçilməlidir")]
        public int? InventoryTypeId { get; set; }
        [ForeignKey("InventoryTypeId")]
        public InventoryType? InventoryType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Status seçilməlidir")]
        public int? DocumentStatusId { get; set; }
        [ForeignKey("DocumentStatusId")]
        public DocumentStatus? DocumentStatus { get; set; }

        public int? ZoneId { get; set; }
        [ForeignKey("ZoneId")]
        public Zone? Zone { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string? EditedByUserId { get; set; }
        public DateTime? EditedAt { get; set; }

        public ICollection<DocumentItem> Items { get; set; } = new List<DocumentItem>();
    }

    public class StatusColor
    {
        public int Id { get; set; }

        [Required]
        public int DocumentStatusId { get; set; }

        [Required]
        [StringLength(20)]
        public string Color { get; set; } = string.Empty;

        [ForeignKey("DocumentStatusId")]
        public DocumentStatus DocumentStatus { get; set; }
    }

    public class Zone
    {

     

        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? Status { get; set; }
    }

    public class Equipment
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? Status { get; set; }
    }

    public class DocumentType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }

    /// <summary>
    /// İnventar Əməliyyat Tipi: Alış, Yerdəyişmə, Təmir/Baxış...
    /// Action: 0 = Təsir Etmir | 1 = Mədaxil (Alış) | 2 = Yerdəyişmə
    /// </summary>
    public class InventoryType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>0=Təsir Etmir, 1=Mədaxil, 2=Yerdəyişmə</summary>
        public int Action { get; set; } = 0;

        [StringLength(300)]
        public string? Description { get; set; }
    }

}
