using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.Departments;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Super-Admin,Admin,HR-Manager")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IToastNotification _toastNotification;
        public DepartmentController(IDepartmentRepository departmentRepository, IToastNotification toastNotification)
        {
            _departmentRepository = departmentRepository;
            _toastNotification = toastNotification;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetDataAsync(PaginationModel model)
        {
            model.SortColumn = "Id";
            var result = await _departmentRepository.GetAllDepartments(model);
            var jsonData = new { draw = model.draw, recordsFiltered = result.FilteredRecord, recordsTotal = result.TotalRecords, data = result.Data };
            return Json(jsonData);
        }

        [HttpGet]
        public async Task<IActionResult> ManageDepartment(long? id)
        {
            var model = new DepartmentModel();
            if (id != null && id > 0)
            {
                var result = await _departmentRepository.GetDepartmentById((long)id);
                if (result.Success)
                {
                    model = result.Data;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageDepartment(DepartmentModel department)
        {
            if (ModelState.IsValid)
            {
                var result = new BaseApiResponse();
                if (department.Id > 0)
                    result = await _departmentRepository.UpdateDepartment(department);
                else
                    result = await _departmentRepository.AddDepartment(department);

                if (result.Success)
                {
                    if (department.Id > 0)
                        _toastNotification.AddSuccessToastMessage("Department Update successfully");
                    else
                    _toastNotification.AddSuccessToastMessage("Department added successfully");

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _toastNotification.AddErrorToastMessage(result.Message);
                }
            }
            return View(department);
        }

        [HttpGet]
        public async Task<IActionResult> ToggleActivation(long id)
        {
            var result = await _departmentRepository.ToggleStatusDepartment(id);
            if (result.Success)
            {
                _toastNotification.AddSuccessToastMessage("Department Status changed successfully");
                return Json(true);
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Department not Deleted");
                return Json(false);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDepartment(long id)
        {
            var result = await _departmentRepository.DeleteDepartment(id);
            if (result.Success)
            {
                _toastNotification.AddSuccessToastMessage("Department Deleted successfully");
                return Json(true);
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Department not Deleted");
                return Json(false);
            }
        }
    }
}
