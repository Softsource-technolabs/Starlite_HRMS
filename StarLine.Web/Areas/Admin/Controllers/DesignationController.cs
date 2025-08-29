using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.Departments;
using StarLine.Infrastructure.Repositories.Designations;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Super-Admin,Admin,HR-Manager")]
    public class DesignationController : Controller
    {
        private readonly IDesignationRepository _designationRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IToastNotification _toastNotification;
        public DesignationController(IDesignationRepository designationRepository, IToastNotification toastNotification, IDepartmentRepository departmentRepository)
        {
            _designationRepository = designationRepository;
            _toastNotification = toastNotification;
            _departmentRepository = departmentRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetDataAsync(PaginationModel model)
        {
            model.SortColumn = "Id";
            var result = await _designationRepository.GetAllDesignations(model);
            var jsonData = new { draw = model.draw, recordsFiltered = result.FilteredRecord, recordsTotal = result.TotalRecords, data = result.Data };
            return Json(jsonData);
        }

        [HttpGet]
        public async Task<IActionResult> ManageDesignation(long? id)
        {
            var department = await _departmentRepository.GetDepartmentList();
            ViewBag.departmentList = department.Data.Select(_ => new SelectListItem
            {
                Text = _.DepartmentName,
                Value = _.Id.ToString()
            }).ToList();
            var model = new DesignationModel();
            if (id != null && id > 0)
            {
                var result = await _designationRepository.GetDesignationById((long)id);
                if (result.Success)
                {
                    model = result.Data;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageDesignation(DesignationModel Designation)
        {
            var department = await _departmentRepository.GetDepartmentList();
            ViewBag.departmentList = department.Data.Select(_ => new SelectListItem
            {
                Text = _.DepartmentName,
                Value = _.Id.ToString()
            }).ToList();
            if (ModelState.IsValid)
            {
                var result = new BaseApiResponse();
                if (Designation.Id > 0)
                    result = await _designationRepository.UpdateDesignation(Designation);
                else
                    result = await _designationRepository.AddDesignation(Designation);
                if (result.Success)
                {
                    if (Designation.Id > 0)
                        _toastNotification.AddSuccessToastMessage("Designation Update successfully");
                    _toastNotification.AddSuccessToastMessage("Designation added successfully");

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _toastNotification.AddErrorToastMessage(result.Message);
                }
            }
            return View(Designation);
        }

        [HttpGet]
        public async Task<IActionResult> ToggleActivation(long id)
        {
            var result = await _designationRepository.ToggleStatusDesignation(id);
            if (result.Success)
            {
                _toastNotification.AddSuccessToastMessage("Designation Status changed successfully");
                return Json(true);
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Designation not Deleted");
                return Json(false);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDesignation(long id)
        {
            var result = await _designationRepository.DeleteDesignation(id);
            if (result.Success)
            {
                _toastNotification.AddSuccessToastMessage("Designation Deleted successfully");
                return Json(true);
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Designation not Deleted");
                return Json(false);
            }
        }
    }
}
