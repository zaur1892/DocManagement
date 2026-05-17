namespace EvtapAz.Models
{
    public class BuildingProject : ILocalizableEntity
    {
        public int Id { get; set; }
        public int BuildingTypeId { get; set; }
        public BuildingType BuildingType { get; set; } = null!;
        public string Name { get; set; } = "";
        public string? NameRu { get; set; }
        public string? NameEn { get; set; }

        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}
