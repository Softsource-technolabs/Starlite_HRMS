using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.LeaveTypes;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Super-Admin,Admin,HR-Manager")]
    public class LeaveTypeController(ILeaveTypeRepository repository, IToastNotification toastNotification) : Controller
    {
        private readonly ILeaveTypeRepository _repository = repository;
        private readonly IToastNotification _toastNotification = toastNotification;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Get(PaginationModel model)
        {
            if (string.IsNullOrEmpty(model.SortColumn))
                model.SortColumn = "Id";
            var result = await _repository.GetAllLeaveTypes(model);
            var jsonData = new { draw = model.draw, recordsFiltered = result.FilteredRecord, recordsTotal = result.TotalRecords, data = result.Data };
            return Json(jsonData);
        }

        [HttpGet]
        public async Task<IActionResult> ManageLeaveType(long? id)
        {
            var model = new LeaveTypeModel();
            if (id != null && id > 0)
            {
                var result = await _repository.GetLeaveTypeById((long)id);
                if (result.Success)
                {
                    model = result.Data;
                }
                else
                {
                    _toastNotification.AddErrorToastMessage(result.Message);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageLeaveType(LeaveTypeModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _repository.AddUpdateLeaveType(model);
                if (result.Success)
                    _toastNotification.AddSuccessToastMessage(result.Message);
                else
                    _toastNotification.AddErrorToastMessage(result.Message);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.OpenOffcanvas = true;
            return Json(false);
        }

        [HttpGet]
        public async Task<IActionResult> ToggleActivation(long id)
        {
            var result = await _repository.ToggleStatusLeaveType(id);
            if (result.Success)
            {
                _toastNotification.AddSuccessToastMessage(result.Message);
                return Json(true);
            }
            else
            {
                _toastNotification.AddErrorToastMessage(result.Message);
                return Json(false);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _repository.DeleteLeaveType(id);
            if (result.Success)
            {
                _toastNotification.AddSuccessToastMessage(result.Message);
                return Json(true);
            }
            else
            {
                _toastNotification.AddErrorToastMessage(result.Message);
                return Json(false);
            }
        }
    }
}
