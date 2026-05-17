namespace EvtapAz.Models
{
    public class District : ILocalizableEntity
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public City City { get; set; } = null!;
        public string Name { get; set; } = "";
        public string? NameRu { get; set; }
        public string? NameEn { get; set; }
        public int SortOrder { get; set; }

        public ICollection<Settlement> Settlements { get; set; } = new List<Settlement>();
        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}
