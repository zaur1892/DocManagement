namespace EvtapAz.Models
{
    public class MetroStation : ILocalizableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? NameRu { get; set; }
        public string? NameEn { get; set; }
        public string? Line { get; set; }
        public string? LineColor { get; set; }
        public int SortOrder { get; set; }

        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}
