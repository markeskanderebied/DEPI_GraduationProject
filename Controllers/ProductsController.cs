using Microsoft.AspNetCore.Mvc;
using DEPI_GraduationProject.Models;
using System.Linq;

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
}
