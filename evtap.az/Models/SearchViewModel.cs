namespace EvtapAz.Models
{
    public class SearchViewModel
    {
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        public int? SettlementId { get; set; }
        public int? MetroStationId { get; set; }
        public int? PropertyTypeId { get; set; }

        public decimal? AreaMin { get; set; }
        public decimal? AreaMax { get; set; }

        public int? RoomMin { get; set; }
        public int? RoomMax { get; set; }

        public int? FloorMin { get; set; }
        public int? FloorMax { get; set; }

        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }

        public string? ListingNumber { get; set; }
        public string? Keyword { get; set; }

        public List<Listing> Results { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
