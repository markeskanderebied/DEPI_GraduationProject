using DEPI_GraduationProject.Models;
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
			// Get the employee's location ID from session
			int? employeeLocationId = HttpContext.Session.GetInt32("EmployeeLocationId");

			// Start with a base query
			IQueryable<Sales> query = _context.Sales;

			// Apply location filter if available
			if (employeeLocationId.HasValue)
			{
				query = query.Where(s => s.location_id == employeeLocationId.Value);
			}

			// Then apply all the includes
			var salesWithIncludes = query
				//.Include(s => s.Location)
				.Include(s => s.Clients)
				.Include(s => s.SaleDetails)
					.ThenInclude(sd => sd.Product)
				.OrderByDescending(s => s.sale_date);

			// Execute the query
			var sales = await salesWithIncludes.ToListAsync();

			return View(sales);
		}
	}
}
