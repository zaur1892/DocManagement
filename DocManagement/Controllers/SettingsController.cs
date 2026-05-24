using DocManagement.Data;
using DocManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly AppDbContext _context;
        public SettingsController(AppDbContext context) => _context = context;

        // ============================================================
        // MAIN SETTINGS PAGE (tabs)
        // ============================================================
        public async Task<IActionResult> Index(string tab = "doctypes")
        {
            ViewBag.ActiveTab = tab;

            var vm = new SettingsViewModel
            {
                DocumentTypes = await _context.DocumentTypes.OrderBy(x => x.Name).ToListAsync(),
                InventoryTypes = await _context.InventoryTypes.OrderBy(x => x.Name).ToListAsync(),
                Equipments = await _context.Equipments.OrderBy(x => x.Name).ToListAsync(),
                Zones = await _context.Zones.OrderBy(x => x.Name).ToListAsync(),
                AppSettings = await _context.AppSettings.OrderBy(x => x.Key).ToListAsync()
            };
            return View(vm);
        }

        // ============================================================
        // DOCUMENT TYPES
        // ============================================================
        public IActionResult CreateDocumentType() => View("DocumentTypes/Create", new DocumentType());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDocumentType(DocumentType model)
        {
            if (!ModelState.IsValid) return View("DocumentTypes/Create", model);
            _context.DocumentTypes.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "doctypes" });
        }

        public async Task<IActionResult> EditDocumentType(int id)
        {
            var item = await _context.DocumentTypes.FindAsync(id);
            if (item == null) return NotFound();
            return View("DocumentTypes/Edit", item);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDocumentType(DocumentType model)
        {
            if (!ModelState.IsValid) return View("DocumentTypes/Edit", model);
            _context.DocumentTypes.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "doctypes" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDocumentType(int id)
        {
            var item = await _context.DocumentTypes.FindAsync(id);
            if (item != null) { _context.DocumentTypes.Remove(item); await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index), new { tab = "doctypes" });
        }

        // ============================================================
        // INVENTORY TYPES
        // ============================================================
        public IActionResult CreateInventoryType() => View("InventoryTypes/Create", new InventoryType());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInventoryType(InventoryType model)
        {
            if (!ModelState.IsValid) return View("InventoryTypes/Create", model);
            _context.InventoryTypes.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "invtypes" });
        }

        public async Task<IActionResult> EditInventoryType(int id)
        {
            var item = await _context.InventoryTypes.FindAsync(id);
            if (item == null) return NotFound();
            return View("InventoryTypes/Edit", item);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInventoryType(InventoryType model)
        {
            if (!ModelState.IsValid) return View("InventoryTypes/Edit", model);
            _context.InventoryTypes.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "invtypes" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInventoryType(int id)
        {
            var item = await _context.InventoryTypes.FindAsync(id);
            if (item != null) { _context.InventoryTypes.Remove(item); await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index), new { tab = "invtypes" });
        }

        // ============================================================
        // EQUIPMENTS
        // ============================================================
        public IActionResult CreateEquipment() => View("Equipments/Create", new Equipment());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEquipment(Equipment model)
        {
            if (!ModelState.IsValid) return View("Equipments/Create", model);
            model.Status = 1;
            _context.Equipments.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "equipments" });
        }

        public async Task<IActionResult> EditEquipment(int id)
        {
            var item = await _context.Equipments.FindAsync(id);
            if (item == null) return NotFound();
            return View("Equipments/Edit", item);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEquipment(Equipment model)
        {
            if (!ModelState.IsValid) return View("Equipments/Edit", model);
            _context.Equipments.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "equipments" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEquipment(int id)
        {
            var item = await _context.Equipments.FindAsync(id);
            if (item != null) { item.Status = item.Status == 1 ? 0 : 1; await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index), new { tab = "equipments" });
        }

        // ============================================================
        // ZONES
        // ============================================================
        public IActionResult CreateZone() => View("Zones/Create", new Zone());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateZone(Zone model)
        {
            if (!ModelState.IsValid) return View("Zones/Create", model);
            model.Status = 1;
            _context.Zones.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "zones" });
        }

        public async Task<IActionResult> EditZone(int id)
        {
            var item = await _context.Zones.FindAsync(id);
            if (item == null) return NotFound();
            return View("Zones/Edit", item);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditZone(Zone model)
        {
            if (!ModelState.IsValid) return View("Zones/Edit", model);
            _context.Zones.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "zones" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleZone(int id)
        {
            var item = await _context.Zones.FindAsync(id);
            if (item != null) { item.Status = item.Status == 1 ? 0 : 1; await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index), new { tab = "zones" });
        }

        // ============================================================
        // APP SETTINGS
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAppSettings(List<AppSetting> settings)
        {
            foreach (var s in settings)
            {
                var existing = await _context.AppSettings.FirstOrDefaultAsync(x => x.Key == s.Key);
                if (existing != null) existing.Value = s.Value;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { tab = "appsettings" });
        }

        // ============================================================
        // AJAX: InventoryType action dəyərini qaytarır (canlı banner üçün)
        // ============================================================
        [AllowAnonymous]
        public IActionResult GetInventoryTypeAction(int id)
        {
            var invType = _context.InventoryTypes.FirstOrDefault(t => t.Id == id);
            if (invType == null)
                return Json(new { action = -1 });
            return Json(new { action = invType.Action, name = invType.Name });
        }
    }

    public class SettingsViewModel
    {
        public List<DocumentType> DocumentTypes { get; set; } = new();
        public List<InventoryType> InventoryTypes { get; set; } = new();
        public List<Equipment> Equipments { get; set; } = new();
        public List<Zone> Zones { get; set; } = new();
        public List<AppSetting> AppSettings { get; set; } = new();
    }
}
