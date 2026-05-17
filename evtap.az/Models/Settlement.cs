namespace EvtapAz.Models
{
    public class Settlement : ILocalizableEntity
    {
        public int Id { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; } = null!;
        public string Name { get; set; } = "";
        public string? NameRu { get; set; }
        public string? NameEn { get; set; }

        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}
