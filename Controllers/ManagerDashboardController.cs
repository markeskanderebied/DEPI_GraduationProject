using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DEPI_GraduationProject.Controllers
{
	[Authorize(Roles = "Manager")]
	public class ManagerDashboardController : Controller
	{


		private readonly AppDbContext _context;

		public ManagerDashboardController(AppDbContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{

			var name = HttpContext.Session.GetString("username");
			var locationId = HttpContext.Session.GetInt32("EmployeeLocationId");

			// Lookup location name from DB
			var locationName = _context.Locations
				.Where(l => l.Id == locationId)
				.Select(l => l.Name)
				.FirstOrDefault();

			// You can use ViewBag or a ViewModel – here's ViewBag for simplicity
			ViewBag.ManagerName = name;
			ViewBag.LocationName = locationName;

			ViewBag.Role = TempData["UserRole"] ?? "Unknown";
			return View();
		}
	}
}