using AfternoonLaugh.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using StarLine.Core.Common;
using StarLine.Core.CommonService;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.Departments;
using StarLine.Infrastructure.Repositories.Designations;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class DesignationController(IDesignationRepository designationRepository, IToastNotification toastNotification, IDepartmentRepository departmentRepository, HierarchyService hierarchyService) : Controller
    {
        private readonly IDesignationRepository _designationRepository = designationRepository;
        private readonly IDepartmentRepository _departmentRepository = departmentRepository;
        private readonly IToastNotification _toastNotification = toastNotification;
        private readonly HierarchyService _hierarchyService = hierarchyService;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetDataAsync(PaginationModel model)
        {
            model.SortColumn = "Id";
            var result = await _designationRepository.GetAllDesignations(model);
            var jsonData = new { draw = model.draw, recordsFiltered = result.recordsFiltered, recordsTotal = result.recordsTotal, data = result.Data };
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

        [HttpGet]
        public async Task<IActionResult> GetHierarchy()
        {
            var hierarchies = await _hierarchyService.GetHierarchyLevelsAsync();
            hierarchies.Add(0, "Select Level");
            return Json(hierarchies.Select(_ => new SelectListItem
            {
                Text = _.Value,
                Value = _.Key.ToString()
            }).OrderBy(_ => Convert.ToInt32(_.Value)).ToList());
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
                var result = await _designationRepository.AddUpdateDesignation(Designation);
                if (result > 0)
                {
                    _toastNotification.AddSuccessToastMessage(ToastrMessages.GetMsg(ToastrModules.Designation, Designation.Id > 0 ? ToastrMessages.Update : ToastrMessages.Add));
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _toastNotification.AddErrorToastMessage(ToastrMessages.GetMsg(ToastrModules.Designation, Designation.Id > 0 ? ToastrMessages.NotUpdate : ToastrMessages.NotAdded));
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
                _toastNotification.AddSuccessToastMessage(ToastrMessages.GetMsg(ToastrModules.Designation, ToastrMessages.Status));
                return Json(true);
            }
            else
            {
                _toastNotification.AddErrorToastMessage(ToastrMessages.GetMsg(ToastrModules.Designation, ToastrMessages.StatusNotChanged));
                return Json(false);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDesignation(long id)
        {
            var result = await _designationRepository.DeleteDesignation(id);
            if (result.Success)
            {
                _toastNotification.AddSuccessToastMessage(ToastrMessages.GetMsg(ToastrModules.Designation, ToastrMessages.Delete));
                return Json(true);
            }
            else
            {
                _toastNotification.AddErrorToastMessage(ToastrMessages.GetMsg(ToastrModules.Designation, ToastrMessages.NotDelete));
                return Json(false);
            }
        }

        [HttpGet]
        public IActionResult GetHierarchyView(Hierarchy hierarchy)
        {
            return PartialView("_ManageHierarchyPartialView", hierarchy);
        }

        [HttpPost]
        public async Task<JsonResult> AddnewHierarchy([FromForm] Hierarchy hierarchy)
        {
            if (hierarchy.Id > 0 && !string.IsNullOrEmpty(hierarchy.Name))
            {
                await _hierarchyService.AddHierarchyLevelAsync(hierarchy.Id, hierarchy.Name);
                _toastNotification.AddSuccessToastMessage(ToastrMessages.GetMsg(ToastrModules.Hierarchy, ToastrMessages.Add));
                return Json(hierarchy.Id);
            }
            else
            {
                _toastNotification.AddErrorToastMessage(ToastrMessages.GetMsg(ToastrModules.Hierarchy, ToastrMessages.NotAdded));
                return Json(0);
            }
        }
    }
}
