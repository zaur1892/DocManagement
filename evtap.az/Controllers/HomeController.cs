using EvtapAz.Data;
using EvtapAz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvtapAz.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(SearchViewModel? search)
        {
            var query = _db.Listings
                .Include(l => l.City)
                .Include(l => l.District)
                .Include(l => l.Settlement)
                .Include(l => l.MetroStation)
                .Include(l => l.PropertyType)
                .Include(l => l.BuildingType)
                .Include(l => l.RepairType)
                .Where(l => l.IsActive && !l.IsFrozen);

            search ??= new SearchViewModel();

            if (search.CityId.HasValue)
                query = query.Where(l => l.CityId == search.CityId);
            if (search.DistrictId.HasValue)
                query = query.Where(l => l.DistrictId == search.DistrictId);
            if (search.SettlementId.HasValue)
                query = query.Where(l => l.SettlementId == search.SettlementId);
            if (search.MetroStationId.HasValue)
                query = query.Where(l => l.MetroStationId == search.MetroStationId);
            if (search.PropertyTypeId.HasValue)
                query = query.Where(l => l.PropertyTypeId == search.PropertyTypeId);
            if (search.RoomMin.HasValue)
                query = query.Where(l => l.RoomMax >= search.RoomMin || l.RoomMin >= search.RoomMin);
            if (search.RoomMax.HasValue)
                query = query.Where(l => l.RoomMin <= search.RoomMax);
            if (search.AreaMin.HasValue)
                query = query.Where(l => l.AreaMax >= search.AreaMin || l.AreaMin >= search.AreaMin);
            if (search.AreaMax.HasValue)
                query = query.Where(l => l.AreaMin <= search.AreaMax);
            if (search.PriceMin.HasValue)
                query = query.Where(l => l.PriceMax >= search.PriceMin || l.PriceMin >= search.PriceMin);
            if (search.PriceMax.HasValue)
                query = query.Where(l => l.PriceMin <= search.PriceMax);
            if (search.FloorMin.HasValue)
                query = query.Where(l => l.FloorMax >= search.FloorMin || l.FloorMin >= search.FloorMin);
            if (search.FloorMax.HasValue)
                query = query.Where(l => l.FloorMin <= search.FloorMax);
            if (!string.IsNullOrWhiteSpace(search.ListingNumber))
                query = query.Where(l => l.ListingNumber.Contains(search.ListingNumber));
            if (!string.IsNullOrWhiteSpace(search.Keyword))
                query = query.Where(l => (l.Notes != null && l.Notes.Contains(search.Keyword))
                                      || l.City.Name.Contains(search.Keyword)
                                      || (l.District != null && l.District.Name.Contains(search.Keyword)));

            search.TotalCount = await query.CountAsync();

            search.Results = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            ViewBag.Cities = await _db.Cities.OrderBy(c => c.SortOrder).ToListAsync();
            ViewBag.PropertyTypes = await _db.PropertyTypes.OrderBy(p => p.SortOrder).ToListAsync();

            return View(search);
        }

        public IActionResult About() => View();

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                Microsoft.AspNetCore.Localization.CookieRequestCultureProvider.DefaultCookieName,
                Microsoft.AspNetCore.Localization.CookieRequestCultureProvider.MakeCookieValue(
                    new Microsoft.AspNetCore.Localization.RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl ?? "/");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }
    }
}
