using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.Departments;
using StarLine.Infrastructure.Repositories.Employees;
using StarLine.Infrastructure.Repositories.Notices;
using StarLine.Web.IdentityServices;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class NoticeController(INoticeRepository repository, IDepartmentRepository departmentRepository, RoleManager<IdentityRole> roleManager, IEmployeeRepository employeeRepository, IToastNotification notification, IHttpContextAccessor httpContextAccessor) : Controller
    {
        private readonly INoticeRepository _repository = repository;
        private readonly IDepartmentRepository _departmentRepository = departmentRepository;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IToastNotification _toastNotification = notification;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Get(PaginationModel model)
        {
            var result = await _repository.GetAllNotices(model);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> ManageNotice(long id = 0)
        {
            ViewBag.NoticeTypes = CommonFunctions.GetEnumSelectList<NoticeType>();
            ViewBag.DeliveryModes = CommonFunctions.GetEnumSelectList<DeliveryMode>();
            ViewBag.AudienceTypes = CommonFunctions.GetEnumSelectList<AudienceType>();
            var model = new NoticeModel();
            if (id > 0)
            {
                var result = await _repository.GetNoticeById(id);
                if (result.Success)
                    model = result.Data;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageNotice(NoticeModel model)
        {
            ViewBag.NoticeTypes = CommonFunctions.GetEnumSelectList<NoticeType>();
            ViewBag.DeliveryModes = CommonFunctions.GetEnumSelectList<DeliveryMode>();
            ViewBag.AudienceTypes = CommonFunctions.GetEnumSelectList<AudienceType>();
            if (ModelState.IsValid)
            {
                var result = await _repository.AddUpdateNotice(model);

                if (result.Success)
                {
                    _toastNotification.AddSuccessToastMessage(result.Message);
                    return RedirectToAction(nameof(Index));
                }
                _toastNotification.AddErrorToastMessage(result.Message);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ToggleStatus(long id) => Json(await _repository.ToggleStatusNotice(id));

        [HttpDelete]
        public async Task<IActionResult> Delete(long id) => Json(await _repository.DeleteNotice(id));

        [HttpGet]
        public async Task<IActionResult> Publish(long id)
        {
            var result = await _repository.GetNoticeById(id);
            if (result.Success)
            {
                var model = result.Data;

                if (model.AudienceType == AudienceType.Department)
                {
                    long departmentId = Convert.ToInt64(model.AudienceTypeValue);
                    var deptResult = await _departmentRepository.GetDepartmentById(departmentId);
                    model.AudienceTypeValue = deptResult.Data.DepartmentName;
                }
                else if (model.AudienceType == AudienceType.Role)
                {
                    var roleResult = await _roleManager.FindByIdAsync(model.AudienceTypeValue);
                    model.AudienceTypeValue = roleResult.Name;
                }
                else if (model.AudienceType == AudienceType.User)
                {
                    var userResult = await _employeeRepository.GetEmployeeById(Convert.ToInt64(model.AudienceTypeValue));
                    model.AudienceTypeValue = userResult.Data.FirstName + " " + userResult.Data.LastName;
                }

                model.FileName = GetAppBaseUrl() + model.FileName;
                return View(result.Data);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult PublishNotice(long id)
        {
            return RedirectToAction(nameof(Index));
        }

        private string GetAppBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            return request != null
                ? $"{request.Scheme}://{request.Host}{request.PathBase}"
                : string.Empty;
        }

        [HttpGet]
        public async Task<JsonResult> GetDepartment(long id)
        {
            var result = await _departmentRepository.GetDepartmentById(id);
            if (result.Success)
            {
                return Json(new { data = result.Data, result = true });
            }
            return Json(new { data = "", result = false });
        }

        [HttpGet]
        public async Task<JsonResult> GetRole(string id)
        {
            var result = await _roleManager.FindByIdAsync(id);
            if (result != null)
            {
                return Json(new { data = result, result = true });
            }
            return Json(new { data = "", result = false });
        }

        [HttpGet]
        public async Task<JsonResult> GetUser(long id)
        {
            var result = await _employeeRepository.GetEmployeeById(id);
            if (result != null)
            {
                return Json(new { data = result.Data, result = true });
            }
            return Json(new { data = "", result = false });
        }
    }
}
