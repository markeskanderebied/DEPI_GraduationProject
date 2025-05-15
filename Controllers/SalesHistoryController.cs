using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEPI_GraduationProject.Controllers
{
    public class SalesHistoryController : Controller
    {
        private readonly AppDbContext _context;

        public SalesHistoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Load Sales with related SaleDetails and Product
            var sales = await _context.Sales
                .Include(s => s.Location)
                .Include(s => s.Clients)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .OrderByDescending(s => s.sale_date)
                .ToListAsync();

            return View(sales);
        }
    }
}
