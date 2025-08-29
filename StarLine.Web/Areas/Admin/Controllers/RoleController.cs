using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Models;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Super-Admin,Admin,HR-Manager")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(_ => new RoleModel
            {
                Id = _.Id,
                Name = _.Name
            }).ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> ManageRole(string id)
        {
            var model = new RoleModel();
            if (!string.IsNullOrEmpty(id))
            {
                var role = await _roleManager.FindByIdAsync(id);
                model.Id = role.Id;
                model.Name = role.Name;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageRole(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                if (!await _roleManager.RoleExistsAsync(model.Name))
                {
                    var role = await _roleManager.FindByIdAsync(model.Id);
                    if (role != null)
                    {
                        role.Name = model.Name;
                        await _roleManager.UpdateAsync(role);
                    }
                }
                var irole = new IdentityRole { Name = model.Name };
                await _roleManager.CreateAsync(irole);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
