using Microsoft.AspNetCore.Mvc;

public class AdminDashboardController : Controller
{
	public IActionResult Index()
	{
		ViewBag.Role = TempData["UserRole"] ?? "Unknown";
		return View();
	}
}
