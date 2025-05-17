using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DEPI_GraduationProject.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace DEPI_GraduationProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles
            .Where(r => r.Id != null && r.Name != null) // Ensure non-null values
            .Select(r => new RoleModel { Id = r.Id!, Name = r.Name! }) // Use null-forgiving operator
            .ToList();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole { Name = model.Name };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
    }
}