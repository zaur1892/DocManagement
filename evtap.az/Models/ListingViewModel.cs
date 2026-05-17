using System.ComponentModel.DataAnnotations;

namespace EvtapAz.Models
{
    public class ListingViewModel
    {
        public int Id { get; set; }

        // Step 1: Məkan
        [Required(ErrorMessage = "Şəhər seçilməlidir")]
        public int CityId { get; set; }
        public int? DistrictId { get; set; }
        public int? SettlementId { get; set; }
        public int? MetroStationId { get; set; }
        public string? Address { get; set; }

        // Step 2: Bina
        [Required(ErrorMessage = "Mənzil tipi seçilməlidir")]
        public int PropertyTypeId { get; set; }
        public int? BuildingTypeId { get; set; }
        public int? BuildingProjectId { get; set; }

        // Step 3: Mənzil məlumatları
        public decimal? AreaMin { get; set; }
        public decimal? AreaMax { get; set; }
        public int? RoomMin { get; set; }
        public int? RoomMax { get; set; }
        public int? FloorMin { get; set; }
        public int? FloorMax { get; set; }
        public int? TotalFloors { get; set; }
        public int? RepairTypeId { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }

        // Step 4: Əlaqə
        [Required(ErrorMessage = "Ad soyad daxil edilməlidir")]
        public string FullName { get; set; } = "";

        [EmailAddress(ErrorMessage = "Düzgün email formatı daxil edin")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Telefon nömrəsi daxil edilməlidir")]
        public string Phone { get; set; } = "";

        public string? Notes { get; set; }
    }
}
