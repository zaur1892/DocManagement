using ClosedXML.Excel;
using DocManagement.Data;
using DocManagement.Models;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System.Security.Claims;
using Document = DocManagement.Models.Document;
using System.Security.Claims;

namespace DocManagement.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly AppDbContext _context;

        public DocumentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Documents
        public IActionResult Index(
            DateTime? startDate,
            DateTime? endDate,
            int? documentTypeId,
            int? statusId)
        {
            // 🔹 AppSettings-dən BeginDate oxu (format: YYYYMMDD, misal: 20260101)
            var beginDateSetting = _context.AppSettings
                .Where(x => x.Key == "BeginDate")
                .Select(x => x.Value)
                .FirstOrDefault();

            var completedStatus = _context.AppSettings
           .Where(x => x.Key == "CompletedStatusCode")
           .Select(x => x.Value)
           .FirstOrDefault();


            if (!string.IsNullOrEmpty(beginDateSetting) && beginDateSetting.Length == 8)
            {
                startDate = DateTime.ParseExact(beginDateSetting, "yyyyMMdd", null);
            }

            // 🔹 End date həmişə bu gün
            endDate ??= DateTime.Today;

            var documents = _context.vw_DocumentList
                .Where(d =>
                    d.CreatedAt.Date >= startDate.Value.Date &&
                    d.CreatedAt.Date <= endDate.Value.Date &&
                    d.StatusCode != completedStatus)
                .AsQueryable();

            if (documentTypeId.HasValue)
                documents = documents.Where(d => d.DocumentTypeId == documentTypeId);

            if (statusId.HasValue)
                documents = documents.Where(d => d.DocumentStatusId == statusId);

            var result = documents
                .OrderByDescending(d => d.CreatedAt)
                .ToList();

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            var columns = ViewBag.Columns as List<DocElement> ?? new List<DocElement>();

            ViewBag.DocumentTypeList = new SelectList(
                _context.DocumentTypes.OrderBy(x => x.Name),
                "Id",
                "Name",
                documentTypeId);

            ViewBag.StatusList = new SelectList(
                _context.DocumentStatus.OrderBy(x => x.Name),
                "Id",
                "Name",
                statusId);

            // 🔥 BURANI ƏLAVƏ ET
            ViewBag.Columns = _context.DocElements
                .Where(x => x.IsVisible)
                .OrderBy(x => x.OrderIndex)
                .ToList();

            return View(result);
        }


        // Excel Export
        public IActionResult ExportToExcel(DateTime? startDate, DateTime? endDate, int? documentTypeId)
    {
        startDate ??= DateTime.Today;
        endDate ??= DateTime.Today;

        var query = _context.Documents
            .Include(d => d.DocumentType)
            .Include(d => d.DocumentStatus)
            .Include(d => d.Items)
            .ThenInclude(i => i.Equipment)
            .Where(d =>
                d.CreatedAt.Date >= startDate.Value.Date &&
                d.CreatedAt.Date <= endDate.Value.Date);

        if (documentTypeId.HasValue)
            query = query.Where(d => d.DocumentTypeId == documentTypeId.Value);

        var data = query.OrderByDescending(d => d.CreatedAt).ToList();

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Tələbnamələr");

        // ========== HEADER ==========
        var headers = new[]
        {
        "ID", "Tələbnamə Tipi", "Tarix", "Status", "Zona", "Avadanlıq", "Qeyd"
    };

        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
        }

        var headerRange = ws.Range(1, 1, 1, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        // ========== DATA ==========
        int row = 2;
        foreach (var d in data)
        {
            ws.Cell(row, 1).Value = d.Id;
            ws.Cell(row, 2).Value = d.DocumentType?.Name;
            ws.Cell(row, 3).Value = d.CreatedAt;
            ws.Cell(row, 4).Value = d.DocumentStatus?.Name;
            ws.Cell(row, 5).Value = string.Join(", ", d.Items.Select(i => i.Location).Distinct());
            ws.Cell(row, 6).Value = string.Join(", ", d.Items.Select(i => i.Equipment?.Name));
            ws.Cell(row, 7).Value = d.Note;

            row++;
        }

        // ========== FORMATTING ==========
        var allRange = ws.Range(1, 1, row - 1, headers.Length);
        allRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        allRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        allRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        allRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

        ws.Column(3).Style.DateFormat.Format = "dd.MM.yyyy";
        ws.Column(7).Style.Alignment.WrapText = true;

        ws.Columns().AdjustToContents();

        // Freeze header
        ws.SheetView.FreezeRows(1);

        using var stream = new MemoryStream();
        wb.SaveAs(stream);
        stream.Position = 0;

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Telebnameler_{DateTime.Now:dd_MM_yyyy}.xlsx"
        );
    }






    // CREATE
    [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            LoadAllCombos();
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,User")]
        public IActionResult Create(Document document)
        {
            if (!ModelState.IsValid)
            {
                LoadAllCombos(
                    document.DocumentTypeId
                );
                return View(document);
            }

            // Note: DocumentItems are handled via form binding if named correctly.

            document.CreatedAt = DateTime.Now;
            document.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _context.Documents.Add(document);
            _context.SaveChanges(); // Əvvəlcə yaz ki, Items-lər DocumentId alsın

            // İnventarla sinxronizasiya (SaveChanges-dən SONRA)
            var savedDoc = _context.Documents
                .Include(d => d.Items)
                .Include(d => d.DocumentStatus)
                .FirstOrDefault(d => d.Id == document.Id);
            if (savedDoc != null) SyncInventory(savedDoc);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }



        // EDIT (GET)
[Authorize(Roles = "Admin")]
public IActionResult Edit(int? id)
{
    if (id == null)
        return NotFound();

    var document = _context.Documents
        .Include(d => d.Items)
        .FirstOrDefault(d => d.Id == id);
    if (document == null)
        return NotFound();

    LoadAllCombos(
        document.DocumentTypeId
    );

    return View(document);
}


        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, Document document)
        {
            if (id != document.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                LoadAllCombos(
                    document.DocumentTypeId
                );
                return View(document);
            }

            var dbDocument = _context.Documents
                .Include(d => d.Items)
                .FirstOrDefault(d => d.Id == id);
            if (dbDocument == null)
                return NotFound();

            // 🗑️ Əvvəlki inventar qeydlərini HƏMİŞƏN təmizlə (tip dəyişsə də, dəyişməsə də)
            // Beləliklə duplicate əmələ gəlmir
            var currentInvType = dbDocument.InventoryTypeId.HasValue
                ? _context.InventoryTypes.FirstOrDefault(t => t.Id == dbDocument.InventoryTypeId)
                : null;

            if (currentInvType != null && currentInvType.Action == 1)
            {
                var inventories = _context.Inventories.ToList();
                
                // İlk növbədə bu sənədə (DocumentId) bağlı olanları tapıb silirik (ən etibarlı yol)
                var exactMatches = inventories.Where(i => i.DocumentId == dbDocument.Id).ToList();
                if (exactMatches.Any())
                {
                    _context.Inventories.RemoveRange(exactMatches);
                    foreach (var m in exactMatches) inventories.Remove(m);
                }
                else
                {
                    // Köhnə sənədlər üçün fallback (DocumentId olmayanlar)
                    foreach (var oldItem in dbDocument.Items.Where(i => i.EquipmentId > 0))
                    {
                        Inventory? invToRemove = null;
                        if (!string.IsNullOrWhiteSpace(oldItem.SerialNumber))
                            invToRemove = inventories.FirstOrDefault(inv => inv.SerialNumber == oldItem.SerialNumber && inv.DocumentId == null);
                        
                        if (invToRemove == null)
                            invToRemove = inventories.FirstOrDefault(inv => inv.EquipmentId == oldItem.EquipmentId && inv.DocumentId == null);

                        if (invToRemove != null)
                        {
                            _context.Inventories.Remove(invToRemove);
                            inventories.Remove(invToRemove);
                        }
                    }
                }
            }

            // 🔴 SAHƏLƏR
            dbDocument.DocumentTypeId = document.DocumentTypeId;
            dbDocument.DocumentStatusId = document.DocumentStatusId;
            dbDocument.ZoneId = document.ZoneId;
            dbDocument.Note = document.Note;
            dbDocument.InventoryTypeId = document.InventoryTypeId;

            // Mövcud itemləri sil, yenilərini əlavə et
            _context.DocumentItems.RemoveRange(dbDocument.Items);
            dbDocument.Items.Clear();

            if (document.Items != null && document.Items.Any())
            {
                var validItems = document.Items.Where(i => i.EquipmentId > 0).ToList();
                foreach (var item in validItems)
                {
                    item.Id = 0;
                    item.DocumentId = dbDocument.Id;
                    dbDocument.Items.Add(item);
                }
            }

            // Yeni InventoryType-a görə inventarı sinxronlaşdır
            SyncInventory(dbDocument);

            // Audit
            dbDocument.EditedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            dbDocument.EditedAt = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        // DELETE
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var document = _context.Documents
                .Include(d => d.DocumentStatus)
                .FirstOrDefault(d => d.Id == id);

            if (document == null) return NotFound();

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var document = _context.Documents.FirstOrDefault(d => d.Id == id);

            if (document == null)
                return NotFound();

            // 🔴 HARD DELETE YOX
            document.DocumentStatusId = 0; // STATUS = 0
            _context.Documents.Update(document);
            
            // Əgər Mədaxildirsə (Action=1), onun yaratdığı inventarları da bazadan silək!
            var invType = document.InventoryTypeId.HasValue 
                ? _context.InventoryTypes.FirstOrDefault(t => t.Id == document.InventoryTypeId) 
                : null;
                
            if (invType != null && invType.Action == 1)
            {
                var invsToRemove = _context.Inventories.Where(i => i.DocumentId == document.Id).ToList();
                if (invsToRemove.Any())
                {
                    _context.Inventories.RemoveRange(invsToRemove);
                }
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        // DETAILS (Audit View-dən)
        public IActionResult Details(int id)
        {
            var auditLogs = _context.DocumentAuditView
                .Where(x => x.DocumentId == id)
                .OrderBy(x => x.LogInsertDateTime)
                .ToList();

            if (!auditLogs.Any())
                return NotFound();

            return View(auditLogs);
        }


        // Helper
        private void LoadAllCombos(int? selectedDocumentTypeId = null)
        {
            ViewBag.StatusList = new SelectList(
                _context.DocumentStatus.OrderBy(x => x.Name),
                "Id",
                "Name"
            );

            ViewBag.ZoneList = new SelectList(
                _context.Zones.Where(z => z.Status == 1).OrderBy(z => z.Name),
                "Id",
                "Name"
            );

            ViewBag.EquipmentList = new SelectList(
                _context.Equipments.Where(e => e.Status == 1).OrderBy(e => e.Name),
                "Id",
                "Name"
            );

            ViewBag.DocumentTypeList = new SelectList(
                _context.DocumentTypes.OrderBy(x => x.Name),
                "Id",
                "Name",
                selectedDocumentTypeId
            );

            // 📦 INVENTORY TYPE (Tələbnamə İnventar Tipi)
            ViewBag.InventoryTypeList = new SelectList(
                _context.InventoryTypes.OrderBy(x => x.Name),
                "Id",
                "Name"
            );
        }

         private void SyncInventory(Document document)
        {
            // 🛑 STATUS YOXLAMASI: Yalnız Tamamlanıb olduqda inventara təsir et!
            var completedStatusCodeSetting = _context.AppSettings.FirstOrDefault(x => x.Key == "CompletedStatusCode")?.Value;
            bool isCompleted = false;
            
            var status = document.DocumentStatus ?? _context.DocumentStatus.FirstOrDefault(s => s.Id == document.DocumentStatusId);
            if (status != null && (status.Name == completedStatusCodeSetting || status.Id == 3))
            {
                isCompleted = true;
            }
            
            if (!isCompleted) return;

            // InventoryType-a bax
            if (document.InventoryTypeId == null) return;
            var invType = _context.InventoryTypes.FirstOrDefault(t => t.Id == document.InventoryTypeId);
            if (invType == null || invType.Action == 0) return; // 0: Təsir Etmir

            if (document.Items == null || !document.Items.Any()) return;

            // Action=1: SerialNumber olmasa da işləyir
            // Action=2: SerialNumber mütləq lazımdır (əslində serial yoxdursa da işləyir)
            var validItems = invType.Action == 1
                ? document.Items.Where(i => i.EquipmentId > 0).ToList()
                : document.Items.Where(i => i.EquipmentId > 0 && !string.IsNullOrWhiteSpace(i.SerialNumber)).ToList();

            // Əgər Action=2 (Yerdəyişmə) üçün serial yoxdursa belə dəyişməyə icazə vermək istəyiriksə:
            if (invType.Action == 2) 
                validItems = document.Items.Where(i => i.EquipmentId > 0).ToList();

            // Bütün inventarları yaddaşa yükləyirik ki, duplikat yoxlaması düzgün işləsin
            var inventories = _context.Inventories.Local.Any() 
                ? _context.Inventories.Local.ToList() 
                : _context.Inventories.ToList();

            foreach (var item in validItems)
            {
                int qty = item.Qty > 0 ? item.Qty : 1;

                if (invType.Action == 1) // 1: Mədaxil (Alış)
                {
                    for (int i = 0; i < qty; i++)
                    {
                        // Mədaxil həmişə İNVENTARA YENİDƏN ƏLAVƏ EDİR
                        var newInv = new Inventory
                        {
                            EquipmentId = item.EquipmentId,
                            SerialNumber = item.SerialNumber,
                            Model = item.Model,
                            CurrentLocation = item.Location,
                            CurrentIpAddress = item.IpAddress,
                            Status = "İstifadədə",
                            DocumentId = document.Id
                        };
                        _context.Inventories.Add(newInv);
                        inventories.Add(newInv); 
                    }
                }
                else if (invType.Action == 2) // 2: Yerдəyişmə
                {
                    for (int i = 0; i < qty; i++)
                    {
                        Inventory? existingInv = null;
                        if (!string.IsNullOrWhiteSpace(item.SerialNumber))
                            existingInv = inventories.FirstOrDefault(inv => inv.SerialNumber == item.SerialNumber);
                        
                        if (existingInv == null)
                            existingInv = inventories.FirstOrDefault(inv => inv.EquipmentId == item.EquipmentId);

                        if (existingInv != null)
                        {
                            existingInv.CurrentLocation = item.Location ?? existingInv.CurrentLocation;
                            existingInv.CurrentIpAddress = item.IpAddress ?? existingInv.CurrentIpAddress;
                            _context.Inventories.Update(existingInv);
                            inventories.Remove(existingInv); // Bir dəfə yerdəyişmə edildisə, eyni obyekti təkrar tapmasın deyə çıxarırıq
                        }
                    }
                }
            }
        }

        public IActionResult DetailsPdf(int id)
        {
            var logs = _context.DocumentAuditView
                .Where(x => x.DocumentId == id)
                .OrderBy(x => x.LogInsertDateTime)
                .ToList();

            if (!logs.Any())
                return NotFound();

            return new ViewAsPdf("DetailsPdf", logs)
        {
                FileName = $"Senəd_{id}.pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Portrait,
                CustomSwitches = "--disable-smart-shrinking"
            };
        }


        public IActionResult Tamamlananlar(
            DateTime? startDate,
            DateTime? endDate,
            int? documentTypeId,
            int? statusId)
        {
            var completedStatus = _context.AppSettings
                .Where(x => x.Key == "CompletedStatusCode")
                .Select(x => x.Value)
                .FirstOrDefault();

            var documents = _context.vw_DocumentList
                .Where(d => d.StatusCode == completedStatus)
                .AsQueryable();

            if (startDate.HasValue)
                documents = documents.Where(d => d.CreatedAt.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                documents = documents.Where(d => d.CreatedAt.Date <= endDate.Value.Date);

            if (documentTypeId.HasValue)
                documents = documents.Where(d => d.DocumentTypeId == documentTypeId);

            if (statusId.HasValue)
                documents = documents.Where(d => d.DocumentStatusId == statusId);

            var result = documents
                .OrderByDescending(d => d.CreatedAt)
                .ToList();

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            // 🔥 BURANI ƏLAVƏ ET
            ViewBag.Columns = _context.DocElements
                .Where(x => x.IsVisible)
                .OrderBy(x => x.OrderIndex)
                .ToList();

            ViewBag.DocumentTypeList = new SelectList(
                _context.DocumentTypes.OrderBy(x => x.Name),
                "Id",
                "Name",
                documentTypeId);

            ViewBag.StatusList = new SelectList(
                _context.DocumentStatus.OrderBy(x => x.Name),
                "Id",
                "Name",
                statusId);

            return View("Index", result);
        }


        public IActionResult Teleblerim(
     DateTime? startDate,
     DateTime? endDate,
     int? documentTypeId,
     int? statusId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!startDate.HasValue)
                startDate = DateTime.Today.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Today;

            var documents = _context.vw_DocumentList
                .Where(d => d.UserId == userId)
                .Where(d =>
                    d.CreatedAt.Date >= startDate.Value.Date &&
                    d.CreatedAt.Date <= endDate.Value.Date)
                .AsQueryable();

            if (documentTypeId.HasValue)
                documents = documents.Where(d => d.DocumentTypeId == documentTypeId);

            if (statusId.HasValue)
                documents = documents.Where(d => d.DocumentStatusId == statusId);

            var result = documents
                .OrderByDescending(d => d.CreatedAt)
                .ToList();

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            // 🔥 BURANI ƏLAVƏ ET
            ViewBag.Columns = _context.DocElements
                .Where(x => x.IsVisible)
                .OrderBy(x => x.OrderIndex)
                .ToList();

            ViewBag.DocumentTypeList = new SelectList(
                _context.DocumentTypes.OrderBy(x => x.Name),
                "Id",
                "Name",
                documentTypeId);

            ViewBag.StatusList = new SelectList(
                _context.DocumentStatus.OrderBy(x => x.Name),
                "Id",
                "Name",
                statusId);

            return View("Index", result);
        }


    }
}