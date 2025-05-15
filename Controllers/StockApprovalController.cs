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
            var request = await _context.StockRequests.FindAsync(id);
            if (request != null && request.Status == "Pending")
            {
                request.Status = "Approved";
                await _context.SaveChangesAsync();
            }
            return PartialView("_StockRequestList", await GetStockRequests());
        }

        // POST: StockApproval/Decline
        [HttpPost]
        public async Task<IActionResult> Decline(int id)
        {
            var request = await _context.StockRequests.FindAsync(id);
            if (request != null && request.Status == "Pending")
            {
                request.Status = "Declined";
                await _context.SaveChangesAsync();
            }
            return PartialView("_StockRequestList", await GetStockRequests());
        }

        private async Task<List<StockRequestViewModel>> GetStockRequests()
        {
            return await (from req in _context.StockRequests
                          join prod in _context.Products on req.ProductCode.Trim().ToLower() equals prod.Code.Trim().ToLower() into prodGroup
                          from prod in prodGroup.DefaultIfEmpty()
                          join fromLoc in _context.Locations on req.FromLocationId equals fromLoc.Id
                          join toLoc in _context.Locations on req.ToLocationId equals toLoc.Id
                          where req.Status == "Pending" || req.Status == "Approved" || req.Status == "Declined"
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