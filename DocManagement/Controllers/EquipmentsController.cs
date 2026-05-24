using DocManagement.Data;
using DocManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EquipmentsController : Controller
    {
        private readonly AppDbContext _context;

        public EquipmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Equipments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Equipments.ToListAsync());
        }

        // GET: Equipments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Equipments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                equipment.Status = 1; // Default status active
                _context.Add(equipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(equipment);
        }

        // GET: Equipments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment == null) return NotFound();

            return View(equipment);
        }

        // POST: Equipments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Equipment equipment)
        {
            if (id != equipment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(equipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(equipment);
        }

        // POST: Equipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment != null)
            {
                equipment.Status = 0; // Soft delete or change status
                _context.Update(equipment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Equipments/Inventory
        public async Task<IActionResult> Inventory(string? search)
        {
            ViewBag.Search = search;

            var inventoryQuery = _context.Inventories
                .Include(i => i.Equipment)
                .Where(i => i.Equipment != null);

            if (!string.IsNullOrWhiteSpace(search))
                inventoryQuery = inventoryQuery.Where(i =>
                    (i.SerialNumber != null && i.SerialNumber.Contains(search)) ||
                    (i.Equipment != null && i.Equipment.Name.Contains(search)));

            var inventoryRaw = await inventoryQuery.ToListAsync();

            // İnventarda mövcud olan DocumentId-lərə uyğun Sənədləri və Zonalarını yükləyirik
            var docIds = inventoryRaw.Where(i => i.DocumentId.HasValue).Select(i => i.DocumentId.Value).Distinct().ToList();
            var documents = await _context.Documents.Include(d => d.Zone).Where(d => docIds.Contains(d.Id)).ToListAsync();

            // Avadanlıq adına görə qruplaşdır
            var inventory = inventoryRaw
                .GroupBy(i => new { i.EquipmentId, EquipmentName = i.Equipment!.Name })
                .Select(g => new InventoryViewModel
                {
                    EquipmentId = g.Key.EquipmentId,
                    EquipmentName = g.Key.EquipmentName,
                    Count = g.Count(),
                    Details = g.Select(d =>
                    {
                        // Əsl Zonanı sənəddən götürürük
                        var doc = d.DocumentId.HasValue ? documents.FirstOrDefault(x => x.Id == d.DocumentId.Value) : null;
                        var realZoneName = doc?.Zone?.Name;

                        return new InventoryDetailViewModel
                        {
                            SerialNumber = d.SerialNumber,
                            Model = d.Model,
                            Location = d.CurrentLocation,
                            ZoneName = realZoneName, // Artıq CurrentLocation yox, əsl zona!
                            IpAddress = d.CurrentIpAddress,
                            Status = d.Status
                        };
                    }).ToList()
                })
                .OrderBy(x => x.EquipmentName)
                .ToList();

            return View(inventory);
        }

        // GET: Equipments/LookupBySerial?serial=SN123
        [AllowAnonymous]
        public IActionResult LookupBySerial(string serial)
        {
            if (string.IsNullOrWhiteSpace(serial))
                return Json(null);

            var inv = _context.Inventories
                .Where(i => i.SerialNumber == serial.Trim())
                .Select(i => new
                {
                    serialNumber = i.SerialNumber,
                    model        = i.Model,
                    location     = i.CurrentLocation,
                    ipAddress    = i.CurrentIpAddress,
                    status       = i.Status,
                    equipmentId  = i.EquipmentId
                })
                .FirstOrDefault();

            return Json(inv);
        }

        public async Task<IActionResult> InventoryPdf(string? search)
        {
            var inventoryRaw = await _context.Inventories
                .Include(i => i.Equipment)
                .Where(i => i.Equipment != null)
                .ToListAsync();

            var docIds = inventoryRaw.Where(i => i.DocumentId.HasValue).Select(i => i.DocumentId.Value).Distinct().ToList();
            var documents = await _context.Documents.Include(d => d.Zone).Where(d => docIds.Contains(d.Id)).ToListAsync();

            var inventory = inventoryRaw
                .GroupBy(i => new { i.EquipmentId, EquipmentName = i.Equipment!.Name })
                .Select(g => new InventoryViewModel
                {
                    EquipmentId = g.Key.EquipmentId,
                    EquipmentName = g.Key.EquipmentName,
                    Count = g.Count(),
                    Details = g.Select(d => {
                        var doc = d.DocumentId.HasValue ? documents.FirstOrDefault(x => x.Id == d.DocumentId.Value) : null;
                        return new InventoryDetailViewModel
                        {
                            SerialNumber = d.SerialNumber,
                            Model = d.Model,
                            Location = d.CurrentLocation,
                            ZoneName = doc?.Zone?.Name,
                            IpAddress = d.CurrentIpAddress,
                            Status = d.Status
                        };
                    }).ToList()
                })
                .OrderBy(x => x.EquipmentName)
                .ToList();

            return new Rotativa.AspNetCore.ViewAsPdf("InventoryPdf", inventory)
            {
                FileName = $"İnventar_{DateTime.Now:dd_MM_yyyy}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                CustomSwitches = "--disable-smart-shrinking"
            };
        }
    }

    public class InventoryViewModel
    {
        public int EquipmentId { get; set; }
        public int Count { get; set; }
        public string? EquipmentName { get; set; }
        public List<InventoryDetailViewModel> Details { get; set; } = new List<InventoryDetailViewModel>();
    }

    public class InventoryDetailViewModel
    {
        public string? SerialNumber { get; set; }
        public string? Model { get; set; }
        public string? ZoneName { get; set; }
        public string? Location { get; set; }
        public string? IpAddress { get; set; }
        public string? Status { get; set; }
    }
}
