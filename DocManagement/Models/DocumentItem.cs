using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocManagement.Models
{
    public class DocumentItem
    {
        public int Id { get; set; }
        
        [ForeignKey("Document")]
        public int DocumentId { get; set; }
        
        [ForeignKey("Equipment")]
        public int EquipmentId { get; set; } 
        
        [StringLength(100)]
        public string? SerialNumber { get; set; }
        
        [StringLength(100)]
        public string? Model { get; set; }
        
        [StringLength(255)]
        public string? Location { get; set; }
        
        [StringLength(50)]
        public string? IpAddress { get; set; }
        
        public int? InventoryId { get; set; } 
        
        // Yeni əlavə edilən Say xanası
        public int Qty { get; set; } = 1;
        
        public Document? Document { get; set; }
        public Equipment? Equipment { get; set; }
    }
}
