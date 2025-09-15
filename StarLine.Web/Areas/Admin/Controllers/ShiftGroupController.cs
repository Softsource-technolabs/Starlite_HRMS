using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.Shifts;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class ShiftGroupController(IShiftGroupRepository repository) : Controller
    {
        private readonly IShiftGroupRepository _repository = repository;

        public async Task<IActionResult> Index()
        {
            var model = await _repository.GetAllShiftGroups();
            return View(model.Data);
        }

        [HttpGet]
        public async Task<IActionResult> ManageShiftGroup(long id)
        {
            ViewBag.RotationTypes = CommonFunctions.GetEnumSelectList<ShiftRotationType>();
            var model = new ShiftGroupModel();
            if (id > 0)
            {
                var result = await _repository.GetShiftGroup(id);
                model = result.Data;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageShiftGroup(ShiftGroupModel model)
        {
            ViewBag.RotationTypes = CommonFunctions.GetEnumSelectList<ShiftRotationType>();
            if (ModelState.IsValid)
            {
                var result = new ApiPostResponse<long>();
                if (model.Id > 0)
                    result = await _repository.UpdateShiftGroup(model.Id, model);
                else
                    result = await _repository.AddShiftGroup(model);

                if (result.Success)
                    return RedirectToAction(nameof(Index));
                else
                    return View(model);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var result = await _repository.ToggleStatus(id);
            return Json(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteShiftGroup(long id)
        {
            var result = await _repository.DeleteShiftGroup(id);
            return Json(result);
        }
    }
}
