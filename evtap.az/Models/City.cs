namespace EvtapAz.Models
{
    public class City : ILocalizableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? NameRu { get; set; }
        public string? NameEn { get; set; }
        public bool HasDistricts { get; set; }
        public bool HasMetro { get; set; }
        public int SortOrder { get; set; }

        public ICollection<District> Districts { get; set; } = new List<District>();
        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}
