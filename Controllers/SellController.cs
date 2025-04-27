using DEPI_GraduationProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEPI_GraduationProject.Models;

public class SellController : Controller
{
	private readonly AppDbContext _context;

	public SellController(AppDbContext context)
	{
		_context = context;
	}

	[HttpGet]
	public IActionResult Index()
	{
		return View();
	}
	/*

	[HttpPost]
	public async Task<IActionResult> SubmitSale(PosSaleViewModel model)
	{
		if (!ModelState.IsValid || model.Products == null || !model.Products.Any())
		{
			ModelState.AddModelError("", "Invalid data submitted.");
			return View(model);
		}

		var sale = new Sales
		{
			ClientName = model.ClientName,
			ClientNumber = model.ClientNumber,
			CarNumber = model.CarNumber,
			SaleDate = DateTime.Now
		};

		_context.Sales.Add(sale);
		await _context.SaveChangesAsync();

		foreach (var item in model.Products)
		{
			var product = await _context.Products.FirstOrDefaultAsync(p => p.Code == item.ProductCode);
			if (product == null)
				continue; // Or handle as error

			var detail = new SalesDetails
			{
				SaleId = sale.Id,
				ProductId = product.Id,
				AdhesiveML = item.AdhesiveAmount
			};

			_context.SalesDetails.Add(detail);
		}

		await _context.SaveChangesAsync();
		return RedirectToAction("Index"); // or a confirmation page
	}


	*/
	/*
	// GET: Sell/BatchCreate?clientId=123
	public IActionResult BatchCreate(int clientId)
	{
		// Use _context instead of db
		var client = _context.Clients.FirstOrDefault(c => c.Id == clientId);
		if (client == null) return NotFound();

		var vm = new BatchSalesEntryViewModel
		{
			ClientId = client.Id,
			ClientName = client.Name,
			ClientNumber = client.Phone,
			CarNumber = client.CarNumber,
		};
		return View(vm);
	}

	// POST: Sell/BatchCreate
	[HttpPost]
	public IActionResult BatchCreate(BatchSalesEntryViewModel model)
	{
		if (ModelState.IsValid)
		{
			foreach (var order in model.Orders)
			{
				// Save each Sale with SaleDetails and AdhesiveUsage here
			}
			return RedirectToAction("Details", "Clients", new { id = model.ClientId });
		}
		return View(model);
	}

	[HttpGet]
	public IActionResult SuggestProductCodes(string term)
	{
		if (string.IsNullOrWhiteSpace(term))
		{
			return Json(new string[0]);
		}

		var codes = _context.Products
			.Where(p => p.Code.StartsWith(term))
			.OrderBy(p => p.Code)
			.Select(p => p.Code)
			.Take(10)
			.ToArray();

		return Json(codes);
	}

	[HttpPost]
	public async Task<IActionResult> SubmitSale(PosSaleViewModel model)
	{
		if (!ModelState.IsValid || model.Products == null || !model.Products.Any())
		{
			// Return with error message
			ModelState.AddModelError("", "Please enter client and at least one product.");
			return View("index", model);
		}

		// Check if client exists or create a new one
		var client = await _context.Clients
			.FirstOrDefaultAsync(c => c.Phone == model.ClientNumber);

		if (client == null)
		{
			client = new Clients
			{
				Name = model.ClientName,
				Phone = model.ClientNumber,
				CarNumber = model.CarNumber,
				CarModel = "" // You can expand your form later to get this
			};
			_context.Clients.Add(client);
			await _context.SaveChangesAsync();
		}

		// Get current user's location from session or fixed logic
		int locationId = GetCurrentLocationId(); // Replace this with your actual logic

		// Create new sale
		var sale = new Sales
		{
			ClientId = client.Id,
			LocationId = locationId,
			SaleDate = DateTime.Now,
			SaleDetails = new List<SalesDetails>(),
			AdhesiveUsages = new List<AdhesiveUsage>()
		};

		foreach (var item in model.Products)
		{
			var product = await _context.Products.FirstOrDefaultAsync(p => p.Code == item.ProductCode);

			if (product == null)
			{
				ModelState.AddModelError("", $"Product with code '{item.ProductCode}' not found.");
				return View("PosSale", model); // return to the form with error
			}

			sale.SaleDetails.Add(new SalesDetails
			{
				ProductId = product.Id,
				Quantity = 1
			});

			sale.AdhesiveUsages.Add(new AdhesiveUsage
			{
				ProductId = product.Id,
				AdhesiveMl = item.AdhesiveAmount
			});
		}


		_context.Sales.Add(sale);
		await _context.SaveChangesAsync();

		return RedirectToAction("Success"); // or your confirmation page
	}

	private int GetCurrentLocationId()
	{
		// Example logic, customize as needed
		return 3;
	}
	*/

	/*
	[HttpPost]
	public async Task<IActionResult> Index(SellViewModel model)
	{
		if (!ModelState.IsValid)
			return View(model);

		// Find the logged-in employee
		var employeeId = HttpContext.Session.GetInt32("EmployeeId");
		if (employeeId == null)
		{
			return RedirectToAction("Login", "Account");
		}

		var employee = await _context.Employees
			.FirstOrDefaultAsync(e => e.Id == employeeId.Value);

		if (employee == null)
		{
			return NotFound("Employee not found.");
		}

		// Find the product
		var product = await _context.Products
			.FirstOrDefaultAsync(p => p.Code == model.WindshieldCode);

		if (product == null)
		{
			ModelState.AddModelError("WindshieldCode", "Product not found.");
			return View(model);
		}

		// Check quantity in inventory
		var inventory = await _context.Inventory
			.FirstOrDefaultAsync(i => i.ProductId == product.Id && i.LocationId == employee.LocationId);

		if (inventory == null || inventory.Quantity < 1)
		{
			ModelState.AddModelError("", "Not enough stock.");
			return View(model);
		}

		// Create a new sale
		var sale = new Sale
		{
			ProductId = product.Id,
			LocationId = employee.LocationId,
			EmployeeId = employee.Id,
			Quantity = 1,
			AdhesiveUsed = model.AdhesiveAmount,
			Date = DateTime.Now,
			CustomerName = model.CustomerName,
			CustomerPhone = model.CustomerPhone,
			CarNumber = model.CarNumber
		};

		_context.Sales.Add(sale);
		inventory.Quantity -= 1;

		await _context.SaveChangesAsync();

		TempData["Success"] = "Sale successfully recorded!";
		return RedirectToAction("Index");
	}*/
}



