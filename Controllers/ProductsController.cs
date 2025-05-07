using Microsoft.AspNetCore.Mvc;
using DEPI_GraduationProject.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class ProductsController : Controller
{
	private readonly AppDbContext _context;

	public ProductsController(AppDbContext context)
	{
		_context = context;
	}

	// 🔍 Endpoint for autocomplete product codes
	[HttpGet]
	public JsonResult SuggestProductCodes(string term)
	{
		var matchingCodes = _context.Products
			.Where(p => p.Code.Contains(term))
			.Select(p => p.Code)
			.Take(10)
			.ToList();

		return Json(matchingCodes);
	}

	// 🏷️ Endpoint to get product name by code
	[HttpGet]
	public JsonResult GetProductNameByCode(string code)
	{
		var product = _context.Products
			.FirstOrDefault(p => p.Code == code);

		if (product != null)
		{
			return Json(new { productName = product.name });
		}

		return Json(new { productName = "" });
	}


	// 📦 Show all products
	public IActionResult Index()
	{
		var products = _context.Products.Include(p => p.Category).ToList();
		return View(products);
	}
	 
	// 🆕 Create form (GET)
	[HttpGet]
	public IActionResult Create()
	{
		ViewData["CategoryId"] = new SelectList(_context.glassfixationCategory, "id", "CatName");
		return View();
	}

	// 🆕 Create action (POST)
	[HttpPost]
	public async Task<IActionResult> Create(Product newProduct)
	{
		if (ModelState.IsValid)
		{
			_context.Products.Add(newProduct);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}
		ViewData["CategoryId"] = new SelectList(_context.glassfixationCategory, "id", "CatName", newProduct.CategoryId);

		return View(newProduct);
	}

	// ✏️ Edit form (GET)
	[HttpGet]
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == null)
			return BadRequest();

		var product = await _context.Products.FindAsync(id);
		if (product == null)
			return NotFound();

		ViewData["CategoryId"] = new SelectList(_context.glassfixationCategory, "id", "CatName", product.CategoryId);
		return View(product);
	}

	// ✏️ Edit action (POST)
	[HttpPost]
	public async Task<IActionResult> Edit(Product updatedProduct)
	{
		Console.WriteLine($"hi");
		if (ModelState.IsValid)
		{
			_context.Entry(updatedProduct).State = EntityState.Modified;
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		// Log validation errors
		var errors = ModelState.Values.SelectMany(v => v.Errors)
									   .Select(e => e.ErrorMessage)
									   .ToList();
		foreach (var error in errors)
		{
			// Replace this with your logging mechanism (e.g., ILogger)
			Console.WriteLine($"Validation Error: {error}");
		}

		ViewData["CategoryId"] = new SelectList(_context.glassfixationCategory, "id", "CatName", updatedProduct.CategoryId);
		return View(updatedProduct);
	}

	// 📄 Product details
	public IActionResult Details(int? id)
	{
		if (id == null)
			return BadRequest();

		var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
		if (product == null)
			return NotFound();

		return View(product);
	}

	// ❌ Delete confirmation (GET)
	[HttpGet]
	public IActionResult Delete(int? id)
	{
		if (id == null)
			return BadRequest();

		var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
		if (product == null)
			return NotFound();

		return View(product);
	}

	// ❌ Delete confirmed (POST)
	[HttpPost, ActionName("Delete")]
	public async Task<IActionResult> DeleteConfirmed(int id)
	{
		var product = await _context.Products.FindAsync(id);
		if (product == null)
			return NotFound();

		_context.Products.Remove(product);
		await _context.SaveChangesAsync();
		return RedirectToAction("Index");
	}
}

