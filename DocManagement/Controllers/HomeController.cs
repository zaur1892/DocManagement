using DocManagement.Data;
using DocManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DocManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, string groupBy = "Day")
        {
            // Default dates
            if (!startDate.HasValue)
            {
                startDate = DateTime.Today.AddDays(-30); // Default last 30 days
            }
            if (!endDate.HasValue)
            {
                endDate = DateTime.Today.AddDays(1).AddTicks(-1); // End of today
            }
            else
            {
                endDate = endDate.Value.Date.AddDays(1).AddTicks(-1); // End of selected day
            }

            var model = new DashboardViewModel
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                GroupBy = groupBy
            };

            // Parametrik: AppSettings-dən "CompletedStatus" açarını oxuyuruq
            // Əgər tapılmazsa, defaul olaraq "Tamamlanıb" sözü ilə axtaracağıq.
            var completedStatusSetting = await _context.AppSettings.FirstOrDefaultAsync(a => a.Key == "CompletedStatus");
            string completedStatusName = completedStatusSetting?.Value ?? "Tamamlanıb";

            // Baza məlumatları
            var documentsQuery = _context.vw_DocumentList
                .Where(d => d.CreatedAt >= startDate && d.CreatedAt <= endDate);

            var documents = await documentsQuery.ToListAsync();

            model.TotalJobs = documents.Count;
            model.CompletedJobs = documents.Count(d => d.StatusName != null && d.StatusName.Contains(completedStatusName, StringComparison.OrdinalIgnoreCase));

            // Statuslara görə qruplaşdırma
            model.JobsByStatus = documents
                .Where(d => !string.IsNullOrEmpty(d.StatusName))
                .GroupBy(d => d.StatusName)
                .ToDictionary(g => g.Key, g => g.Count());

            // Avadanlıqlara görə qruplaşdırma
            model.JobsByEquipment = documents
                .Where(d => !string.IsNullOrEmpty(d.EquipmentName))
                .GroupBy(d => d.EquipmentName)
                .ToDictionary(g => g.Key, g => g.Count());

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
