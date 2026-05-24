using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocManagement.Models
{
    public class Inventory
    {
        public int Id { get; set; }

        // Mədaxil edən sənədin ID-si (Əlavə və silinmədə kömək üçün)
        public int? DocumentId { get; set; }
        
        [ForeignKey("Equipment")]
        public int EquipmentId { get; set; } 
        
        [StringLength(100)]
        public string? SerialNumber { get; set; }
        
        [StringLength(100)]
        public string? Model { get; set; }
        
        [StringLength(255)]
        public string? CurrentLocation { get; set; }
        
        [StringLength(50)]
        public string? CurrentIpAddress { get; set; }
        
        [StringLength(50)]
        public string? Status { get; set; } 
        
        public Equipment? Equipment { get; set; }
    }
}
