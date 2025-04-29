using DEPI_GraduationProject.ViewModels;
using DEPI_GraduationProject.Models;    // Namespace for your models
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEPI_GraduationProject.Controllers
{
	public class SalesController : Controller
	{
		private readonly AppDbContext _context; // Your DbContext

		public SalesController(AppDbContext context)
		{
			_context = context;
		}

		// GET: /Sales/PosSale
		[HttpGet]
		public IActionResult PosSale()
		{
			var viewModel = new PosSaleViewModel();
			
			return View(viewModel);
		}

		// POST: /Sales/SubmitSale
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SubmitSale(PosSaleViewModel model)
		{
			// Basic validation from annotations
			if (!ModelState.IsValid)
			{
				// Manually check if any valid products were added if view allows submitting empty table initially
				if (model.Products == null || !model.Products.Any(p => !string.IsNullOrWhiteSpace(p.ProductCode) ))
				{
					ModelState.AddModelError("Products", "At least one valid product entry is required.");
					// Return here if you want to enforce having items client-side validation might miss
					// return View("PosSale", model);
				}
				// Still return if other model errors exist
				return View("PosSale", model);
			}

			// --- Determine LocationId ---


			//int locationId = HttpContext.Session.GetInt32("EmployeeLocationId")
			// ?? throw new Exception("Employee Location not found in session.");

			int locationId = 0;

			// --- Client Handling Logic ---
			// Assuming model.ClientNumber maps to Clients.Phone
			if (string.IsNullOrWhiteSpace(model.ClientNumber))
			{
				ModelState.AddModelError("ClientNumber", "Client phone number is required.");
				return View("PosSale", model);
			}

			Clients client = await _context.Clients // Use exact model name 'Clients'
										.FirstOrDefaultAsync(c => c.Phone == model.ClientNumber);

			if (client == null)
			{
				// Client not found, create a new one using data from ViewModel
				client = new Clients
				{
					Name = model.ClientName,
					Phone = model.ClientNumber, // Store number in Phone field
					CarNumber = model.CarNumber
				};
				_context.Clients.Add(client);
				try
				{
					// Save now to get the Client ID before creating the Sale
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateException dbEx)
				{
					// Log dbEx.InnerException
					ModelState.AddModelError("", "Failed to save new client information. " + dbEx.InnerException?.Message ?? dbEx.Message);
					return View("PosSale", model);
				}
				catch (Exception ex) // Catch other potential errors during client save
				{
					// Log ex
					ModelState.AddModelError("", "An unexpected error occurred while saving client information. " + ex.Message);
					return View("PosSale", model);
				}
			}
			// We now have a valid client.Id
			int clientId = client.Id;


			// --- Data Processing Logic ---
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					// 1. Create the main Sale record
					var sale = new Sales // Use 'Sales' model name
					{
						LocationId = locationId,
						ClientId = clientId,
						SaleDate = DateTime.UtcNow,
						SaleDetails = new List<SalesDetails>(), // Initialize collections
						
					};
					_context.Sales.Add(sale);
					// Save now to get sale.Id
					await _context.SaveChangesAsync();

					// 2. Process each product line item from the ViewModel
					foreach (var item in model.Products)
					{
						// Skip invalid/empty rows more robustly
						if (string.IsNullOrWhiteSpace(item.ProductCode) )
						{
							continue;
						}

						// Find the product by its Code
						var product = await _context.Products
							.FirstOrDefaultAsync(p => p.Code == item.ProductCode);

						if (product == null)
						{
							ModelState.AddModelError("", $"Product with code '{item.ProductCode}' not found.");
							await transaction.RollbackAsync();
							return View("PosSale", model);
						}

						// --- Inventory Check ---
						// Find inventory record for this product at this location
						// Using exact model names: Inventory, Product_id, Location_id
						var inventoryItem = await _context.Inventory
													.FirstOrDefaultAsync(inv => inv.Product_id == product.Id && inv.Location_id == locationId);

						// --- !!! CRITICAL LOGIC ISSUE !!! ---
						// The 'Inventory' model tracks 'Quantity' (item count).
						// The 'AdhesiveUsage' model tracks 'Amount' (volume, ml).
						// We CANNOT directly compare inventoryItem.Quantity (e.g., number of tubes)
						// with item.AdhesiveAmount (e.g., 50 ml) without knowing how many ml
						// are in one quantity unit of the adhesive product.
						//
						// TODO: Requires clarification or model change:
						// 1. Does Product model need an 'MlPerUnit' property for adhesives?
						// 2. Should Inventory track volume instead of quantity for liquids?
						//
						// For now, we can only check if *any* inventory record exists,
						// but cannot perform a meaningful stock *level* check for adhesive volume.
						// We will proceed WITHOUT blocking the sale based on this check.

						if (inventoryItem == null)
						{
							// Log this maybe? Or decide if having an inventory record is mandatory
							// ModelState.AddModelError("", $"Inventory record not found for product {product.Code} at location {locationId}.");
							// await transaction.RollbackAsync();
							// return View("PosSale", model);
							// For now, let's allow proceeding even without inventory record, but this should be reviewed.
						}
						else
						{
							// We have an inventoryItem.Quantity, but cannot compare it meaningfully to item.AdhesiveAmount (ml)
							// Example: if (inventoryItem.Quantity < ???) { Error }
							// Cannot implement this check correctly with current models.
						}


						// Create the Sale Detail record
						var saleDetail = new SalesDetails // Use 'SalesDetails' model name
						{
							// SaleId will be set by EF relationship fixup if added to sale.SaleDetails
							// SaleId = sale.Id,
							ProductId = product.Id,
							Quantity = 1 // *** ASSUMPTION: Quantity is 1 per line item ***
						};
						sale.SaleDetails.Add(saleDetail); // Add to Sale's collection
						/*
						// Create Adhesive Usage record
						if (item.AdhesiveAmount > 0)
						{
							var adhesiveUsage = new AdhesiveUsage
							{
								// SaleId will be set by EF relationship fixup
								// SaleId = sale.Id,
								ProductId = product.Id, // *** ASSUMPTION: Linking usage to the product in the row ***
								Amount = (double)item.AdhesiveAmount, // Cast Decimal to Double for model
																	  // UsageDate could be set here or rely on SaleDate relation
							};
							sale.AdhesiveUsages.Add(adhesiveUsage); // Add to Sale's collection
						}*/
					} // End of product loop

					// Check if any valid items were actually added to the sale
					if (!sale.SaleDetails.Any())
					{
						ModelState.AddModelError("", "The sale must contain at least one valid product item.");
						await transaction.RollbackAsync();
						return View("PosSale", model);
					}

					// 3. Save all changes (Sale, new Client, SaleDetails, AdhesiveUsages)
					// EF Core tracks changes and relationships. One SaveChangesAsync should suffice here.
					await _context.SaveChangesAsync();

					// 4. Commit the transaction
					await transaction.CommitAsync();

					// 5. Redirect after successful submission
					TempData["SuccessMessage"] = $"Sale #{sale.Id} submitted successfully!";
					return RedirectToAction("PosSale");

				}
				catch (DbUpdateException dbEx)
				{
					await transaction.RollbackAsync();
					// Log dbEx.InnerException
					ModelState.AddModelError("", "Database error saving sale: " + dbEx.InnerException?.Message ?? dbEx.Message);
					return View("PosSale", model);
				}
				catch (Exception ex) // Catch other unexpected errors
				{
					await transaction.RollbackAsync();
					// Log ex
					ModelState.AddModelError("", "An unexpected error occurred: " + ex.Message);
					return View("PosSale", model);
				}
			} // End of transaction using block
		}
	}
}