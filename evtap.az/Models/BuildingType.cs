namespace EvtapAz.Models
{
    public class BuildingType : ILocalizableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? NameRu { get; set; }
        public string? NameEn { get; set; }
        public bool HasProjects { get; set; }

        public ICollection<BuildingProject> Projects { get; set; } = new List<BuildingProject>();
        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}
