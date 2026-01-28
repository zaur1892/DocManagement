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
//goz deneme
        // GET: Documents
        public IActionResult Index(
            DateTime? startDate,
            DateTime? endDate,
            int? documentTypeId,
            int? statusId
        )
        {
            // Default tarixlər → bu gün yox son 1 Ay
            if (!startDate.HasValue)
                startDate = DateTime.Today.AddMonths(-1);

            if (!endDate.HasValue)
                endDate = DateTime.Today;


            var documents = _context.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.DocumentStatus)
                    .ThenInclude(s => s.StatusColor)
                .Include(d => d.Zone)
                .Include(d => d.Equipment)
                .Where(d => d.DocumentStatusId != 0)
                .Where(d =>
                    d.CreatedAt.Date >= startDate.Value.Date &&
                    d.CreatedAt.Date <= endDate.Value.Date)
                .AsQueryable();

            // 🔹 DOCUMENT TYPE FILTER
            if (documentTypeId.HasValue)
            {
                documents = documents.Where(d => d.DocumentTypeId == documentTypeId.Value);
            }

            // 🔹 STATUS FILTER
            if (statusId.HasValue)
            {
                documents = documents.Where(d => d.DocumentStatusId == statusId.Value);
            }

            var result = documents
                .OrderByDescending(d => d.CreatedAt)
                .ToList();

            // 🔹 View üçün saxla
            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

            // 🔹 Document Type Combo
            ViewBag.DocumentTypeList = new SelectList(
                _context.DocumentTypes.OrderBy(x => x.Name),
                "Id",
                "Name",
                documentTypeId
            );

            // 🔹 Status Combo (VACİB – NullReference OLMAZ)
            ViewBag.StatusList = new SelectList(
                _context.DocumentStatus.OrderBy(x => x.Name),
                "Id",
                "Name",
                statusId
            );

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
            .Include(d => d.Zone)
            .Include(d => d.Equipment)
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
            ws.Cell(row, 5).Value = d.Zone?.Name;
            ws.Cell(row, 6).Value = d.Equipment?.Name;
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
            ViewBag.StatusList = new SelectList(_context.DocumentStatus, "Id", "Name");
            ViewBag.ZoneList = new SelectList(_context.Zones, "Id", "Name");
            ViewBag.EquipmentList = new SelectList(_context.Equipments, "Id", "Name");

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
                    document.DocumentTypeId,
                    document.ZoneId,
                    document.EquipmentId
                );
                return View(document);
            }

            document.CreatedAt = DateTime.Now;
            document.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _context.Documents.Add(document);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }



        // EDIT (GET)
[Authorize(Roles = "Admin")]
public IActionResult Edit(int? id)
{
    if (id == null)
        return NotFound();

    var document = _context.Documents.FirstOrDefault(d => d.Id == id);
    if (document == null)
        return NotFound();

    LoadAllCombos(
        document.DocumentTypeId,
        document.ZoneId,
        document.EquipmentId
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
                    document.DocumentTypeId,
                    document.ZoneId,
                    document.EquipmentId
                );
                return View(document);
            }

            var dbDocument = _context.Documents.FirstOrDefault(d => d.Id == id);
            if (dbDocument == null)
                return NotFound();

            // 🔴 SAHƏLƏR
            //dbDocument.Title = document.Title;
            dbDocument.DocumentTypeId = document.DocumentTypeId;
            dbDocument.DocumentStatusId = document.DocumentStatusId;
            dbDocument.ZoneId = document.ZoneId;
            dbDocument.EquipmentId = document.EquipmentId;
            dbDocument.Note = document.Note;

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
        private void LoadAllCombos(int? selectedDocumentTypeId = null, int? selectedZoneId = null, int? selectedEquipmentId = null)
        {
            ViewBag.StatusList = new SelectList(
                _context.DocumentStatus.OrderBy(x => x.Name),
                "Id",
                "Name"
            );

            ViewBag.ZoneList = new SelectList(
                _context.Zones.Where(z => z.Status == 1).OrderBy(z => z.Name),
                "Id",
                "Name",
                selectedZoneId
            );

            ViewBag.EquipmentList = new SelectList(
                _context.Equipments.Where(e => e.Status == 1).OrderBy(e => e.Name),
                "Id",
                "Name",
                selectedEquipmentId
            );

            // 🔴 DOCUMENT TYPE (Tələbnamə tipi)
            ViewBag.DocumentTypeList = new SelectList(
                _context.DocumentTypes.OrderBy(x => x.Name),
                "Id",
                "Name",
                selectedDocumentTypeId
            );
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





public IActionResult Teleblerim(
    DateTime? startDate,
    DateTime? endDate,
    int? documentTypeId,
    int? statusId)
    {
        // 👤 Login olan user
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 📅 Default → son 1 ay
        if (!startDate.HasValue)
            startDate = DateTime.Today.AddMonths(-1);

        if (!endDate.HasValue)
            endDate = DateTime.Today;

        var documents = _context.Documents
            .Include(d => d.DocumentType)
            .Include(d => d.DocumentStatus)
                .ThenInclude(s => s.StatusColor)
            .Include(d => d.Zone)
            .Include(d => d.Equipment)
            .Where(d => d.UserId == userId) // 🔥 ƏSAS ŞƏRT
            .Where(d =>
                d.CreatedAt.Date >= startDate.Value.Date &&
                d.CreatedAt.Date <= endDate.Value.Date)
            .AsQueryable();

        if (documentTypeId.HasValue)
            documents = documents.Where(d => d.DocumentTypeId == documentTypeId.Value);

        if (statusId.HasValue)
            documents = documents.Where(d => d.DocumentStatusId == statusId.Value);

        var result = documents
            .OrderByDescending(d => d.CreatedAt)
            .ToList();

        // ViewBag-lar (Index-də nə var idisə EYNİSİ)
        ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

        ViewBag.DocumentTypeList = new SelectList(
            _context.DocumentTypes.OrderBy(x => x.Name),
            "Id",
            "Name",
            documentTypeId
        );

        ViewBag.StatusList = new SelectList(
            _context.DocumentStatus.OrderBy(x => x.Name),
            "Id",
            "Name",
            statusId
        );

        // 🔁 EYNİ VIEW istifadə olunur
        return View("Index", result);
    }


}
}
