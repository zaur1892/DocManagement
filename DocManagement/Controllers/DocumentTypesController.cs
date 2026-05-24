using DocManagement.Data;
using DocManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DocumentTypesController : Controller
    {
        private readonly AppDbContext _context;

        public DocumentTypesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DocumentTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.DocumentTypes.OrderBy(x => x.Name).ToListAsync());
        }

        // GET: DocumentTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DocumentTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentType documentType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(documentType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(documentType);
        }

        // GET: DocumentTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var documentType = await _context.DocumentTypes.FindAsync(id);
            if (documentType == null) return NotFound();
            return View(documentType);
        }

        // POST: DocumentTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocumentType documentType)
        {
            if (id != documentType.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(documentType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(documentType);
        }
    }
}
