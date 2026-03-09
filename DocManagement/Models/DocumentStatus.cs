using System.ComponentModel.DataAnnotations;

namespace DocManagement.Models
{
    public class DocumentStatus
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public StatusColor StatusColor { get; set; }
    }

}
