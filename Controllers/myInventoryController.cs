using DEPI_GraduationProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEPI_GraduationProject.Models;

public class myInventoryController : Controller
{
	private readonly AppDbContext _context;

	public myInventoryController(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IActionResult> Index()
	{
        var userName = User.Identity?.Name;

        if (string.IsNullOrEmpty(userName))
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _context.AspNetUsers
            .OfType<ApplicationUser>()
            .FirstOrDefaultAsync(u => u.UserName == userName);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        var location_id = user.Location_id;


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
