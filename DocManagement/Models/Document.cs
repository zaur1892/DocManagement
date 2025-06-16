using System.ComponentModel.DataAnnotations;

namespace DocManagement.Models
{
    public class Document
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Document Number")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Yeni əlavə etdiyin sütun:
        public DateTime TrainArrivalTime { get; set; }
    }
}
