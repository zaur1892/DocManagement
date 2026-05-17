namespace EvtapAz.Models
{
    public class PropertyType : ILocalizableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? NameRu { get; set; }
        public string? NameEn { get; set; }
        public string? Icon { get; set; }
        public int SortOrder { get; set; }

        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}
