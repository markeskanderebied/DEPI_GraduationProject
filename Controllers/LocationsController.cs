using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEPI_GraduationProject.Models;

namespace DEPI_GraduationProject.Controllers
{
	public class LocationsController : Controller
	{
		private readonly AppDbContext _context;

		public LocationsController(AppDbContext context)
		{
			_context = context;
		}

		// GET: Locations
		public async Task<IActionResult> Index()
		{
			return View(await _context.Locations.ToListAsync());
		}

		// GET: Locations/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
				return NotFound();

			var location = await _context.Locations.FirstOrDefaultAsync(m => m.Id == id);
			if (location == null)
				return NotFound();

			return View(location);
		}

		// GET: Locations/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Locations/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Name,Type")] Location location)
		{
			if (ModelState.IsValid)
			{
				_context.Add(location);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(location);
		}

		// GET: Locations/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
				return NotFound();

			var location = await _context.Locations.FindAsync(id);
			if (location == null)
				return NotFound();

			return View(location);
		}

		// POST: Locations/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type")] Location location)
		{
			if (id != location.Id)
				return NotFound();

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(location);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!LocationExists(location.Id))
						return NotFound();
					else
						throw;
				}
				return RedirectToAction(nameof(Index));
			}
			return View(location);
		}

		// GET: Locations/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
				return NotFound();

			var location = await _context.Locations.FirstOrDefaultAsync(m => m.Id == id);
			if (location == null)
				return NotFound();

			return View(location);
		}

		// POST: Locations/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var location = await _context.Locations.FindAsync(id);
			if (location != null)
			{
				_context.Locations.Remove(location);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}

		private bool LocationExists(int id)
		{
			return _context.Locations.Any(e => e.Id == id);
		}
	}
}
