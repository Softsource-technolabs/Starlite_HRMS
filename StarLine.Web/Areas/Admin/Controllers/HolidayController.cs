using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.Holidays;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Super-Admin,Admin,HR-Manager")]
    public class HolidayController(IHolidayRepository repository, IToastNotification toastNotification) : Controller
    {
        private readonly IHolidayRepository _repository = repository;
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
            var result = await _repository.GetAllHolidays(model);
            var jsonData = new { draw = model.draw, recordsFiltered = result.FilteredRecord, recordsTotal = result.TotalRecords, data = result.Data };
            return Json(jsonData);
        }

        [HttpGet]
        public async Task<IActionResult> ManageHoliday(long? id)
        {
            var model = new HolidayModel();
            if (id != null && id > 0)
            {
                var result = await _repository.GetHolidayById((long)id);
                if (result.Success)
                    model = result.Data;
                else
                    _toastNotification.AddErrorToastMessage("Holiday not found");
            }
            return PartialView("_HolidayPartialView", model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageHoliday(HolidayModel model)
        {
            if (ModelState.IsValid)
            {
                var result = new BaseApiResponse();
                if (model.Id > 0)
                    result = await _repository.UpdateHoliday(model);
                else
                    result = await _repository.AddHoliday(model);

                if (result.Success)
                    _toastNotification.AddSuccessToastMessage(model.Id > 0 ? "Holiday details updated" : "Holday added successfully");
                else
                    _toastNotification.AddErrorToastMessage("Error while adding holiday details");
                return RedirectToAction(nameof(Index));
            }
            ViewBag.OpenOffcanvas = true;
            return Json(false);
        }

        [HttpGet]
        public async Task<IActionResult> ToggleActivation(long id)
        {
            var result = await _repository.ToggleStatusHoliday(id);
            if (result.Success)
            {
                _toastNotification.AddSuccessToastMessage("Holiday Status changed successfully");
                return Json(true);
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Holiday not Deleted");
                return Json(false);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _repository.DeleteHoliday(id);
            if (result.Success)
            {
                _toastNotification.AddSuccessToastMessage("Holiday Deleted successfully");
                return Json(true);
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Holiday not Deleted");
                return Json(false);
            }
        }
    }
}
