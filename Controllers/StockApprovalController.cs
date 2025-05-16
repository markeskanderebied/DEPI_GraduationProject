using DEPI_GraduationProject.Models;
using DEPI_GraduationProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEPI_GraduationProject.Controllers
{
	public class StockApprovalController : Controller
	{
		private readonly AppDbContext _context;

		public StockApprovalController(AppDbContext context)
		{
			_context = context;
		}

		// GET: StockApproval
		public async Task<IActionResult> Index()
		{
			var requests = await GetStockRequests();
			Console.WriteLine($"Number of requests returned: {requests.Count}");
			if (requests.Count == 0)
			{
				Console.WriteLine("No requests found in Index action.");
			}
			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				return PartialView("_StockRequestList", requests);
			}
			return View(requests);
		}

		// GET: StockApproval/Search
		public async Task<IActionResult> Search(string searchTerm)
		{
			var requests = await GetStockRequests();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				searchTerm = searchTerm.ToLower();
				requests = requests.Where(r =>
					r.ProductCode.ToLower().Contains(searchTerm) ||
					r.FromLocationName.ToLower().Contains(searchTerm) ||
					r.ToLocationName.ToLower().Contains(searchTerm) ||
					r.Status.ToLower().Contains(searchTerm))
					.ToList();
			}

			return PartialView("_StockRequestList", requests);
		}
		// POST: StockApproval/Approve
		[HttpPost]
		public async Task<IActionResult> Approve(int id)
		{
			var employeeLocationId = HttpContext.Session.GetInt32("EmployeeLocationId");
			if (employeeLocationId == null)
			{
				return Json(new { success = false, message = "You must be logged in to approve requests." });
			}

			var request = await _context.StockRequests.FindAsync(id);
			if (request == null || request.Status != "Pending" || request.FromLocationId != employeeLocationId)
			{
				return Json(new { success = false, message = "Invalid request or insufficient permissions." });
			}

			// Find the product by code
			var product = await _context.Products.FirstOrDefaultAsync(p =>
				p.Code.Trim().ToLower() == request.ProductCode.Trim().ToLower());

			if (product == null)
			{
				return Json(new { success = false, message = "Product not found in system." });
			}

			// Use a transaction to ensure all database operations succeed or fail together
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					// Check if there's sufficient inventory at the source location
					var sourceInventory = await _context.Inventory
						.FirstOrDefaultAsync(i =>
							i.Location_id == request.FromLocationId &&
							i.Product_id == product.Id);

					if (sourceInventory == null || sourceInventory.Quantity < request.Quantity)
					{
						// Not enough inventory - return error without changing status
						return Json(new
						{
							success = false,
							message = $"Insufficient inventory to fulfill this request. Available: {sourceInventory?.Quantity ?? 0}, Requested: {request.Quantity}",
							requestId = id
						});
					}

					// Deduct inventory from source location
					sourceInventory.Quantity -= request.Quantity;
					_context.Inventory.Update(sourceInventory);

					// Check if destination location already has inventory for this product
					var destinationInventory = await _context.Inventory
						.FirstOrDefaultAsync(i =>
							i.Location_id == request.ToLocationId &&
							i.Product_id == product.Id);

					if (destinationInventory != null)
					{
						// Update existing inventory
						destinationInventory.Quantity += request.Quantity;
						_context.Inventory.Update(destinationInventory);
					}
					else
					{
						// Create new inventory record for destination
						_context.Inventory.Add(new Models.Inventory
						{
							Location_id = request.ToLocationId,
							Product_id = product.Id,
							Quantity = request.Quantity,
							Shelf = "Transfer" // Default shelf for transferred items - you may want to modify this
						});
					}

					// Approve the request
					request.Status = "Approved";
					_context.StockRequests.Update(request);

					// Save all changes
					await _context.SaveChangesAsync();

					// Commit transaction
					await transaction.CommitAsync();

					return Json(new { success = true });
				}
				catch (Exception ex)
				{
					// Roll back transaction on error
					await transaction.RollbackAsync();
					return Json(new { success = false, message = $"Error processing inventory transfer: {ex.Message}" });
				}
			}
		}
		// POST: StockApproval/Decline
		[HttpPost]
		public async Task<IActionResult> Decline(int id)
		{
			var employeeLocationId = HttpContext.Session.GetInt32("EmployeeLocationId");
			if (employeeLocationId == null)
			{
				return RedirectToAction("Index", "Home");
			}

			var request = await _context.StockRequests.FindAsync(id);
			if (request != null && request.Status == "Pending" && request.FromLocationId == employeeLocationId)
			{
				request.Status = "Declined";
				await _context.SaveChangesAsync();
			}
			return PartialView("_StockRequestList", await GetStockRequests());
		}

		private async Task<List<StockRequestViewModel>> GetStockRequests()
		{
			// Get the employee's location ID from the session
			var employeeLocationId = HttpContext.Session.GetInt32("EmployeeLocationId");

			if (employeeLocationId == null)
			{
				return new List<StockRequestViewModel>();
			}

			// Only show requests where the employee's location is the FROM location
			return await (from req in _context.StockRequests
						  join prod in _context.Products on req.ProductCode.Trim().ToLower() equals prod.Code.Trim().ToLower() into prodGroup
						  from prod in prodGroup.DefaultIfEmpty()
						  join fromLoc in _context.Locations on req.FromLocationId equals fromLoc.Id
						  join toLoc in _context.Locations on req.ToLocationId equals toLoc.Id
						  where (req.Status == "Pending" || req.Status == "Approved" || req.Status == "Declined")
							  && req.FromLocationId == employeeLocationId // Only from the employee's location
						  select new StockRequestViewModel
						  {
							  RequestId = req.RequestId,
							  FromLocationName = fromLoc.Name,
							  ToLocationName = toLoc.Name,
							  ProductName = prod != null ? prod.name : "Unknown",
							  ProductCode = prod != null ? prod.Code : "Unknown",
							  Quantity = req.Quantity,
							  RequestDate = req.RequestDate,
							  Status = req.Status
						  }).ToListAsync();
		}
	}
}