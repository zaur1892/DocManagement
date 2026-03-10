using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

    namespace DocManagement.Models
    {
        public class DocumentListView
        {
            public int Id { get; set; }

            // Document əsas sahələri
            public DateTime CreatedAt { get; set; }
            public string Note { get; set; }
            public string StatusCode { get; set; }  // EnumDocStatuses.Tamamlanıb və s.

            // Foreign Key-lar
            public int? DocumentTypeId { get; set; }
            public int? DocumentStatusId { get; set; }
            public int? ZoneId { get; set; }
            public int? EquipmentId { get; set; }

            // İstifadəçi
            public string UserId { get; set; }

            // Join-lardan gələn sahələr
            public string? DocumentTypeName { get; set; }
            public string StatusName { get; set; }
            public string StatusColor { get; set; }  // Hex color string
            public string ZoneName { get; set; }
            public string EquipmentName { get; set; }

        public string? CustomFieldName1 { get; set; }
        public string? CustomFieldName2 { get; set; }
        public string? CustomFieldName3 { get; set; }
    }
    }
