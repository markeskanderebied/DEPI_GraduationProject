using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DEPI_GraduationProject.Controllers
{
    public class ViewOtherInventoryController : Controller
    {
        private readonly AppDbContext _context;

        public ViewOtherInventoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var locations = await _context.Locations.ToListAsync();
            return View(locations);
        }

        public async Task<IActionResult> ByLocation(int id)
        {
            var inventory = await _context.Inventory
                .Where(i => i.Location_id == id)
                .Include(i => i.Product)
                .Include(i => i.Location)
                .ToListAsync();

            ViewBag.Location = await _context.Locations.FindAsync(id);

            return View(inventory);
        }
    }
}
