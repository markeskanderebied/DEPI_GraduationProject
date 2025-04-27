using DEPI_GraduationProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEPI_GraduationProject.Models;
using DEPI_GraduationProject.ViewModels;

public class myInventoryController : Controller
{
	private readonly AppDbContext _context;

	public myInventoryController(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IActionResult> Index()
	{
		var employeeId = HttpContext.Session.GetInt32("EmployeeId");

		if (employeeId == null)
		{
			return RedirectToAction("Login", "Account"); // Or return Unauthorized()
		}

		var employee = await _context.Employees
			.FirstOrDefaultAsync(e => e.Id == employeeId);

		if (employee == null)
		{
			return NotFound("Employee not found.");
		}

		var location_id = employee.Location_id;

		var inventoryList = await _context.Inventory
			.Include(i => i.Location)
			.Include(i => i.Product)
			 .ThenInclude(p => p.Category)
			.Where(i => i.Location_id == location_id)
			.Select(i => new myInventoryViewModel
			{
				ProductCode = i.Product.Code,
				LocationName = i.Location.Name,
				ProductName = i.Product.name,
				ProductType = i.Product.type,
				ProductColor = i.Product.color,
				Quantity = i.Quantity,
				Price = i.Product.Price,
				TotalPrice = i.Product.Price + (i.Product.Category != null ? i.Product.Category.fixation_cost : 0),
				Shelf = i.Shelf
			})
			.ToListAsync();

		return View(inventoryList);
	}

}
