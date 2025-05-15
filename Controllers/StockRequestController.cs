using DEPI_GraduationProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DEPI_GraduationProject.Models;


namespace DEPI_GraduationProject.Controllers
{
    public class StockRequestController : Controller
    {
        private readonly AppDbContext _context;

        public StockRequestController(AppDbContext context)
        {
            _context = context;
        }

        // GET: StockRequest/Create
        public IActionResult Create()
        {
            ViewBag.Locations = new SelectList(_context.Locations, "Id", "Name");
            ViewBag.ProductCodes = new SelectList(_context.Products, "Code", "Code");
            return View();
        }

        // POST: StockRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FromLocationId,ToLocationId,ProductCode,Quantity")] StockRequest request)
        {
            if (ModelState.IsValid)
            {
                // Validate FromLocationId and ToLocationId are different
                if (request.FromLocationId == request.ToLocationId)
                {
                    ModelState.AddModelError("ToLocationId", "From and To locations must be different.");
                }

                // Validate ProductCode exists
                var productExists = await _context.Products.AnyAsync(p => p.Code == request.ProductCode);
                if (!productExists)
                {
                    ModelState.AddModelError("ProductCode", "Invalid product code.");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        request.RequestDate = DateTime.Now;
                        request.Status = "Pending";

                        _context.Add(request);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "An error occurred while saving the request. Please try again.");
                    }
                }
            }

            ViewBag.Locations = new SelectList(_context.Locations, "Id", "Name");
            ViewBag.ProductCodes = new SelectList(_context.Products, "Code", "Code");
            return View(request);
        }

        // GET: StockRequest/Index
        public async Task<IActionResult> Index(string searchTerm)
        {
            var query = from req in _context.StockRequests
                        join fromLoc in _context.Locations on req.FromLocationId equals fromLoc.Id into fromLocGroup
                        from fromLoc in fromLocGroup.DefaultIfEmpty()
                        join toLoc in _context.Locations on req.ToLocationId equals toLoc.Id into toLocGroup
                        from toLoc in toLocGroup.DefaultIfEmpty()
                        join prod in _context.Products on req.ProductCode equals prod.Code into prodGroup
                        from prod in prodGroup.DefaultIfEmpty()
                        where req.Status == "Pending"
                        select new StockRequestViewModel
                        {
                            RequestId = req.RequestId,
                            FromLocationName = fromLoc != null ? fromLoc.Name : "Unknown",
                            ToLocationName = toLoc != null ? toLoc.Name : "Unknown",
                            ProductName = prod != null ? prod.name : "Unknown", // Assuming PascalCase 'Name'
                            Quantity = req.Quantity,
                            RequestDate = req.RequestDate,
                            Status = req.Status
                        };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                string lowered = searchTerm.ToLower();
                query = query.Where(r =>
                    (r.FromLocationName ?? "Unknown").ToLower().Contains(lowered) ||
                    (r.ToLocationName ?? "Unknown").ToLower().Contains(lowered) ||
                    (r.ProductName ?? "Unknown").ToLower().Contains(lowered) ||
                    r.Status.ToLower().Contains(lowered));
            }

            var results = await query.OrderByDescending(r => r.RequestDate).ToListAsync();
            return View(results);
        }
    }
}