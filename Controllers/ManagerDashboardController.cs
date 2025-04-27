using Microsoft.AspNetCore.Mvc;

public class ManagerDashboardController : Controller
{
	public IActionResult Index()
	{
		ViewBag.Role = TempData["UserRole"] ?? "Unknown";
		return View();
	}
}
