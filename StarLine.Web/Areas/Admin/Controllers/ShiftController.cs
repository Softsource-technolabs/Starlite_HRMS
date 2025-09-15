using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using NuGet.Protocol.Core.Types;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.Shifts;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class ShiftController(IShiftRepository shiftRepository, IShiftGroupRepository shiftGroupRepository,IToastNotification toastNotification) : Controller
    {
        private readonly IShiftGroupRepository _shiftGroupRepository = shiftGroupRepository;
        private readonly IShiftRepository _shiftRepository = shiftRepository;
        private readonly IToastNotification _toastNotification = toastNotification;
        public async Task<IActionResult> Index()
        {
            var model = await _shiftGroupRepository.GetAllShiftGroups();
            return View(model.Data);
        }

        [HttpGet]
        public IActionResult ManageShift(long shiftGroupId)
        {
            ViewBag.RotationTypes = CommonFunctions.GetEnumSelectList<ShiftRotationType>();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ManageShift(ShiftWizardModel model)
        {
            var groups = await _shiftGroupRepository.GetAllShiftGroups();
            ViewBag.shiftGroups = groups.Data.Select(_ => new SelectListItem { Value = _.Id.ToString(), Text = _.GroupName }).ToList();
            if (ModelState.IsValid)
            {
                var result = await _shiftRepository.AddorUpdateShift(model);
                if (result.Success)
                {
                    return Json(new { result = result.Success, message = result.Message });
                }
                return Json(new { result = result.Success, message = result.Message });
            }
            return Json(new { result = false, message = "error while processing request" });
        }
    }
}