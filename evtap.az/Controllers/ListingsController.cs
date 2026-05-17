using EvtapAz.Data;
using EvtapAz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EvtapAz.Controllers
{
    [Authorize]
    public class ListingsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public ListingsController(AppDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: /Listings/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadViewBags();
            return View(new ListingViewModel());
        }

        // POST: /Listings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadViewBags();
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            var count = await _db.Listings.CountAsync() + 1;
            var listingNumber = $"EVT-{DateTime.Now.Year}-{count:D5}";

            var listing = new Listing
            {
                UserId = userId,
                ListingNumber = listingNumber,
                CityId = model.CityId,
                DistrictId = model.DistrictId,
                SettlementId = model.SettlementId,
                MetroStationId = model.MetroStationId,
                Address = model.Address,
                PropertyTypeId = model.PropertyTypeId,
                BuildingTypeId = model.BuildingTypeId,
                BuildingProjectId = model.BuildingProjectId,
                AreaMin = model.AreaMin,
                AreaMax = model.AreaMax,
                RoomMin = model.RoomMin,
                RoomMax = model.RoomMax,
                FloorMin = model.FloorMin,
                FloorMax = model.FloorMax,
                TotalFloors = model.TotalFloors,
                RepairTypeId = model.RepairTypeId,
                PriceMin = model.PriceMin,
                PriceMax = model.PriceMax,
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Notes = model.Notes,
                IsActive = true,
                IsFrozen = false,
                CreatedAt = DateTime.Now
            };

            _db.Listings.Add(listing);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Elanınız uğurla yaradıldı! Elan nömrəniz: {listingNumber}";
            return RedirectToAction("MyListings");
        }

        // GET: /Listings/MyListings
        public async Task<IActionResult> MyListings()
        {
            var userId = _userManager.GetUserId(User);
            var listings = await _db.Listings
                .Include(l => l.City)
                .Include(l => l.District)
                .Include(l => l.PropertyType)
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
            return View(listings);
        }

        // POST: Toggle Freeze
        [HttpPost]
        public async Task<IActionResult> ToggleFreeze(int id)
        {
            var userId = _userManager.GetUserId(User);
            var listing = await _db.Listings.FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
            if (listing == null) return NotFound();

            listing.IsFrozen = !listing.IsFrozen;
            listing.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            TempData["Success"] = listing.IsFrozen ? "Elan donduruldu." : "Elan yenidən aktivləşdirildi.";
            return RedirectToAction("MyListings");
        }

        // POST: Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var listing = await _db.Listings.FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
            if (listing == null) return NotFound();

            listing.IsActive = false;
            listing.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Elan ləğv edildi.";
            return RedirectToAction("MyListings");
        }

        // GET: /Listings/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var listing = await _db.Listings
                .Include(l => l.City)
                .Include(l => l.District)
                .Include(l => l.Settlement)
                .Include(l => l.MetroStation)
                .Include(l => l.PropertyType)
                .Include(l => l.BuildingType)
                .Include(l => l.BuildingProject)
                .Include(l => l.RepairType)
                .FirstOrDefaultAsync(l => l.Id == id && l.IsActive && !l.IsFrozen);

            if (listing == null) return NotFound();
            return View(listing);
        }

        // AJAX: Get districts by city
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetDistricts(int cityId)
        {
            var districts = await _db.Districts
                .Where(d => d.CityId == cityId)
                .OrderBy(d => d.SortOrder)
                .Select(d => new { d.Id, d.Name })
                .ToListAsync();
            return Json(districts);
        }

        // AJAX: Get settlements by district
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetSettlements(int districtId)
        {
            var settlements = await _db.Settlements
                .Where(s => s.DistrictId == districtId)
                .OrderBy(s => s.Name)
                .Select(s => new { s.Id, s.Name })
                .ToListAsync();
            return Json(settlements);
        }

        // AJAX: Get building projects by type
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetBuildingProjects(int buildingTypeId)
        {
            var projects = await _db.BuildingProjects
                .Where(p => p.BuildingTypeId == buildingTypeId)
                .OrderBy(p => p.Name)
                .Select(p => new { p.Id, p.Name })
                .ToListAsync();
            return Json(projects);
        }

        private async Task LoadViewBags()
        {
            ViewBag.Cities = new SelectList(
                await _db.Cities.OrderBy(c => c.SortOrder).ToListAsync(), "Id", "Name");
            ViewBag.PropertyTypes = await _db.PropertyTypes.OrderBy(p => p.SortOrder).ToListAsync();
            ViewBag.BuildingTypes = new SelectList(
                await _db.BuildingTypes.ToListAsync(), "Id", "Name");
            ViewBag.RepairTypes = new SelectList(
                await _db.RepairTypes.ToListAsync(), "Id", "Name");
            ViewBag.MetroStations = new SelectList(
                await _db.MetroStations.OrderBy(m => m.SortOrder).ToListAsync(), "Id", "Name");
        }
    }
}
