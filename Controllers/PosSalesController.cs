using DEPI_GraduationProject.Models;
using DEPI_GraduationProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEPI_GraduationProject.Controllers
{
	public class PosSalesController : Controller
	{
		private readonly ILogger<PosSalesController> _logger;
		private readonly AppDbContext _context;

		public PosSalesController(
			ILogger<PosSalesController> logger,
			AppDbContext context)
		{
			_logger = logger;
			_context = context;
		}

		// GET: PosSales
		public IActionResult Index()
		{
			// Get the user's location ID (you might get this from session or claims)
			int locationId = GetCurrentUserLocationId();

			var viewModel = new PosSaleViewModel
			{

				LocationId = locationId
			};

			return View(viewModel);
		}

		// POST: PosSales/SubmitSale
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SubmitSale(PosSaleViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("Index", model);
			}

			try
			{
				// Check if there are any products added
				if (model.Products == null || !model.Products.Any())
				{
					ModelState.AddModelError("", "Please add at least one product to the sale.");
					return View("Index", model);
				}

				// Begin transaction
				using var transaction = await _context.Database.BeginTransactionAsync();

				try
				{
					// 1. Check if client exists, if not create a new one
					var client = await _context.Clients
						.FirstOrDefaultAsync(c => c.Phone == model.ClientNumber && c.CarNumber == model.CarNumber);

					if (client == null)
					{
						// Create new client
						client = new Clients
						{
							Name = model.ClientName,
							Phone = model.ClientNumber,
							CarNumber = model.CarNumber,
							//CarModel = model.CarModel ?? "Unknown" // If you have this field in your viewmodel
						};

						_context.Clients.Add(client);
						await _context.SaveChangesAsync();
					}

					// 2. Create the sales record
					var sale = new Sales
					{
						ClientId = client.Id,
						LocationId = model.LocationId,
						SaleDate = DateTime.Now
					};

					_context.Sales.Add(sale);
					await _context.SaveChangesAsync();

					// 3. Create sales details for each product
					var adhesiveUsages = new List<AdhesiveUsage>();

					foreach (var item in model.Products)
					{
						// Get product by code
						var product = await _context.Products
							.FirstOrDefaultAsync(p => p.Code == item.ProductCode);

						if (product == null)
						{
							throw new Exception($"Product with code {item.ProductCode} not found.");
						}

						// Check inventory
						var inventory = await _context.Inventory
							.FirstOrDefaultAsync(i => i.Product_id == product.Id && i.Location_id == model.LocationId);

						if (inventory == null || inventory.Quantity < 1)
						{
							throw new Exception($"Product {product.name} is out of stock at this location.");
						}

						// Add sales detail
						var salesDetail = new SalesDetails
						{
							SaleId = sale.Id,
							ProductId = product.Id,
							Quantity = 1 // Default to 1 unit per product line
						};

						_context.SalesDetails.Add(salesDetail);

						// Record adhesive usage if applicable
						if (item.AdhesiveAmount > 0)
						{
							var adhesiveUsage = new AdhesiveUsage
							{
								SaleId = sale.Id,
								Amount = (double)item.AdhesiveAmount,
								ProductId = product.Id,
								//Date = DateTime.Now
							};

							adhesiveUsages.Add(adhesiveUsage);
						}

						// Update inventory
						inventory.Quantity -= 1;
						_context.Inventory.Update(inventory);
					}

					// Add adhesive usages
					if (adhesiveUsages.Any())
					{
						_context.AdhesiveUsage.AddRange(adhesiveUsages);
					}

					await _context.SaveChangesAsync();
					await transaction.CommitAsync();

					TempData["SuccessMessage"] = $"Sale #{sale.Id} successfully recorded.";
					return RedirectToAction("SaleConfirmation", new { id = sale.Id });
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					throw ex; // Rethrow to be caught by outer try-catch
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating sale");
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Index", model);
			}
		}

		// GET: PosSales/SaleConfirmation/5
		public async Task<IActionResult> SaleConfirmation(int id)
		{
			var sale = await _context.Sales
				.Include(s => s.Clients)
				.Include(s => s.SaleDetails)
					.ThenInclude(sd => sd.Product)
				.Include(s => s.AdhesiveUsages)
				.FirstOrDefaultAsync(s => s.Id == id);

			if (sale == null)
			{
				return NotFound();
			}

			return View(sale);
		}

		// Helper method to get the current user's location ID
		private int GetCurrentUserLocationId()
		{
			
			int? employeeLocationId = HttpContext.Session.GetInt32("EmployeeLocationId");

			return (int)employeeLocationId;
		}
	}
}