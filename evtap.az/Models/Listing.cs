using System.ComponentModel.DataAnnotations;

namespace EvtapAz.Models
{
    public class Listing
    {
        public int Id { get; set; }

        [Required]
        public string ListingNumber { get; set; } = "";

        public string? UserId { get; set; }

        // --- Məkan ---
        [Required]
        public int CityId { get; set; }
        public City City { get; set; } = null!;

        public int? DistrictId { get; set; }
        public District? District { get; set; }

        public int? SettlementId { get; set; }
        public Settlement? Settlement { get; set; }

        public int? MetroStationId { get; set; }
        public MetroStation? MetroStation { get; set; }

        public string? Address { get; set; }

        // --- Əmlak növü ---
        [Required]
        public int PropertyTypeId { get; set; }
        public PropertyType PropertyType { get; set; } = null!;

        // --- Bina məlumatları ---
        public int? BuildingTypeId { get; set; }
        public BuildingType? BuildingType { get; set; }

        public int? BuildingProjectId { get; set; }
        public BuildingProject? BuildingProject { get; set; }

        // --- Mənzil məlumatları ---
        public decimal? AreaMin { get; set; }
        public decimal? AreaMax { get; set; }

        public int? RoomMin { get; set; }
        public int? RoomMax { get; set; }

        public int? FloorMin { get; set; }
        public int? FloorMax { get; set; }

        public int? TotalFloors { get; set; }

        public int? RepairTypeId { get; set; }
        public RepairType? RepairType { get; set; }

        // --- Qiymət ---
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }

        // --- Əlaqə ---
        [Required]
        public string FullName { get; set; } = "";

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string Phone { get; set; } = "";

        public string? Notes { get; set; }

        // --- Status ---
        public bool IsActive { get; set; } = true;
        public bool IsFrozen { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
