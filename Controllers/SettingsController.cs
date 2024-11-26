using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FISTNESSGYM.Data;
using FISTNESSGYM.Models.database;

namespace FISTNESSGYM.Controllers
{
    public class SettingsController : Controller
    {
        private readonly databaseContext _context;

        public SettingsController(databaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Logs()
        {
            var logs = await _context.Logs
                .OrderBy(l => l.CreatedAt)  // Sortowanie logów od najnowszych
                .ToListAsync();

            return View(logs);
        }
    }

}
