using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocManagement.Models
{
    public class Document
    {
        public int Id { get; set; }
        // Yeni sahə
        // ✅ DOCUMENT TYPE
        public int? DocumentTypeId { get; set; }

        [ForeignKey("DocumentTypeId")]
        public DocumentType? DocumentType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Status seçilməlidir")]
        public int? DocumentStatusId { get; set; }

        [ForeignKey("DocumentStatusId")]
        public DocumentStatus? DocumentStatus { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        // User və audit sahələri
        public string UserId { get; set; } = string.Empty;
        public string? EditedByUserId { get; set; }
        public DateTime? EditedAt { get; set; }

        // 🔴 ZONA & AVADANLIQ
        public int? ZoneId { get; set; }
        public int? EquipmentId { get; set; }

        [ForeignKey("ZoneId")]
        public Zone? Zone { get; set; }

        [ForeignKey("EquipmentId")]
        public Equipment? Equipment { get; set; }
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
        public string Name { get; set; } // Məsələn: "Tələbnamə", "Qeydiyyat", və s.

        public ICollection<Document> Documents { get; set; }
    }

}
