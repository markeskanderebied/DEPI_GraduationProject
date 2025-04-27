using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using DEPI_GraduationProject.Models; // adjust based on your namespace
using System.Linq;
using System;

public class AuthController : Controller
{
	private readonly AppDbContext _context;
	private readonly PasswordHasher<object> _hasher;

	public AuthController(AppDbContext context)
	{
		_context = context;
		_hasher = new PasswordHasher<object>();
	}

	[HttpGet]
	public IActionResult Login()
	{
		return View();
	}

	[HttpPost]
	public IActionResult Login(string username, string password)
	{
		var employee = _context.Employees.FirstOrDefault(e => e.Username == username && e.is_active);

		if (employee == null)
		{
			ViewBag.Error = "Invalid username or password";
			return View();
		}

		var result = _hasher.VerifyHashedPassword(null, employee.Password_hash, password);
		if (result == PasswordVerificationResult.Failed)
		{
			ViewBag.Error = "Invalid username or password";
			return View();
		}

		// Store role + name in session
		HttpContext.Session.SetString("username", employee.Name);
		HttpContext.Session.SetString("role", employee.Role);
		HttpContext.Session.SetInt32("EmployeeId", employee.Id);
		HttpContext.Session.SetInt32("EmployeeLocationId", (int)employee.Location_id);


		// Redirect to appropriate dashboard
		if (employee.Role == "Admin")
			return RedirectToAction("index", "AdminDashboard");
		else
			return RedirectToAction("index", "ManagerDashboard");
	}

	public IActionResult Logout()
	{
		HttpContext.Session.Clear();
		return RedirectToAction("Login");
	}
}
