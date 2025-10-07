using AfternoonLaugh.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Repositories.Departments;
using StarLine.Infrastructure.Repositories.Designations;
using StarLine.Infrastructure.Repositories.Employees;
using StarLine.Web.IdentityServices;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IToastNotification _toastNotification;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDesignationRepository _designationRepository;
        private readonly IUserRepository _userRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserSession _userSession;
        private readonly string _userAvtarlocation;
        public EmployeeController(IEmployeeRepository employeeRepository, IToastNotification toastNotification, IDepartmentRepository departmentRepository, IDesignationRepository designationRepository, IUserRepository userRepository, RoleManager<IdentityRole> roleManager,IWebHostEnvironment env, IUserSession userSession, IHttpContextAccessor contextAccessor)
        {
            _employeeRepository = employeeRepository;
            _toastNotification = toastNotification;
            _departmentRepository = departmentRepository;
            _designationRepository = designationRepository;
            _contextAccessor = contextAccessor;
            _userRepository = userRepository;
            _userSession = userSession;
            _roleManager = roleManager;

            _userAvtarlocation = Path.Combine(env.WebRootPath, "UserAvtars");

            if (!Directory.Exists(_userAvtarlocation))
            {
                Directory.CreateDirectory(_userAvtarlocation);
            }
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Get(PaginationModel model)
        {
            if (string.IsNullOrEmpty(model.SortColumn))
                model.SortColumn = "Id";
            var result = await _employeeRepository.GetAllEmployees(model);
            var jsonData = new { draw = model.draw, recordsFiltered = result.recordsFiltered, recordsTotal = result.recordsTotal, data = result.Data };
            return Json(jsonData);
        }

        [HttpGet]
        public async Task<IActionResult> ManageEmployee(long? id)
        {
            var department = await _departmentRepository.GetDepartmentList();
            ViewBag.departmentList = department.Data.Select(_ => new SelectListItem
            {
                Text = _.DepartmentName,
                Value = _.Id.ToString()
            }).ToList();

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.rolesList = roles.Select(_ => new SelectListItem
            {
                Text = _.Name,
                Value = _.Id
            }).ToList();

            var managers = await _employeeRepository.GetManagersList();
            ViewBag.managerList = managers.Data.Select(_ => new SelectListItem
            {
                Text = _.FirstName+" "+_.LastName,
                Value = _.Id.ToString()
            }).ToList();

            ViewBag.Genders = CommonFunctions.GetEnumSelectList<Genders>();
            ViewBag.EmployementType = CommonFunctions.GetEnumSelectList<EmploymentType>();
            ViewBag.BloodGroups = CommonFunctions.GetEnumSelectList<BloodGroup>();
            var model = new EmployeeModel();
            if (id != null && id > 0)
            {
                var result = await _employeeRepository.GetEmployeeById((long)id);
                if (result.Success)
                {
                    model = result.Data;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageEmployee(EmployeeModel model)
        {
            // Get Dropdown filleds
            var department = await _departmentRepository.GetDepartmentList();
            ViewBag.departmentList = department.Data.Select(_ => new SelectListItem
            {
                Text = _.DepartmentName,
                Value = _.Id.ToString()
            }).ToList();

            var designation = await _designationRepository.GetDesignationList();
            ViewBag.designationList = designation.Data.Select(_ => new SelectListItem
            {
                Text = _.DesignationName,
                Value = _.Id.ToString()
            }).ToList();

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.rolesList = roles.Select(_ => new SelectListItem
            {
                Text = _.Name,
                Value = _.Id
            }).ToList();

            var managers = await _employeeRepository.GetManagersList();
            ViewBag.managerList = managers.Data.Select(_ => new SelectListItem
            {
                Text = _.FirstName + " " + _.LastName,
                Value = _.Id.ToString()
            }).ToList();

            ViewBag.Genders = CommonFunctions.GetEnumSelectList<Genders>();
            ViewBag.EmployementType = CommonFunctions.GetEnumSelectList<EmploymentType>();
            ViewBag.BloodGroups = CommonFunctions.GetEnumSelectList<BloodGroup>();

            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    var checkUserRole = await _userRepository.CheckUserRole(model.AspNetUserId, model.RoleId);
                    if (checkUserRole)
                    {
                        model.UserImages = string.IsNullOrEmpty(model.UserImages) ? AvtarService.GenerateAvatar(model.FirstName + " " + model.LastName, _userAvtarlocation, model.AspNetUserId) : model.UserImages;
                        var result = await _employeeRepository.AddUpdateEmployee(model);
                        _toastNotification.AddSuccessToastMessage(ToastrMessages.GetMsg(ToastrModules.Employee, ToastrMessages.Update));
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _toastNotification.AddSuccessToastMessage(ToastrMessages.GetMsg(ToastrModules.Employee, ToastrMessages.Save));
                    }
                }
                else
                {
                    var appUser = await _userRepository.RegisterUser(new RegisterModel { EmailAddress = model.Email, MobileNo = model.PhoneNumber, RoleId = model.RoleId,FullName = model.FirstName+" "+model.LastName });
                    if (appUser.Success)
                    {
                        model.AspNetUserId = appUser.Data.Id;
                        //Create User Image if user not upload Image
                        model.UserImages = string.IsNullOrEmpty(model.UserImages) ? AvtarService.GenerateAvatar(model.FirstName + " " + model.LastName, _userAvtarlocation, model.AspNetUserId) : model.UserImages;
                        var result = await _employeeRepository.AddUpdateEmployee(model);
                        _toastNotification.AddSuccessToastMessage("Employee added successfully");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Error while creating User Please try again later");
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetDesignation(long id)
        {
            var model = await _designationRepository.GetDesignationByDepartment(id);
            return Json(model);
        }

        [HttpGet]
        public async Task<IActionResult> ToggleActivation(long id)
        {
            var result = await _employeeRepository.ToggleStatusEmployee(id);
            if (result.Success)
            {
                _toastNotification.AddSuccessToastMessage("Employee Status changed successfully");
                return Json(true);
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Employee not Deleted");
                return Json(false);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(long id)
        {
            var result = await _employeeRepository.DeleteEmployee(id);
            if (result.Success)
            {
                _toastNotification.AddSuccessToastMessage("Employee Deleted successfully");
                return Json(true);
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Employee not Deleted");
                return Json(false);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetReportingManager(long designationId)
        {
            var managers = await _employeeRepository.GetReportingManagers(designationId);
            return Json(managers);
        }

        public async Task<IActionResult> ViewProfile()
        {
            var response = await _employeeRepository.GetEmployeeDetailsById(_userSession.Current.UserId);
            string imageUrl = "/UserAvtars/default.png";
            if (!string.IsNullOrEmpty(response.UserImages))
            {
                imageUrl = "/UserAvtars/" + response.UserImages;
            }
            var request = _contextAccessor.HttpContext?.Request;
            string imagefilePath = $"{request.Scheme}://{request.Host}" + imageUrl;
            response.UserImages = imagefilePath;
            return View(response);
        }

        [HttpGet]
        public async Task<JsonResult> CheckEmployeeCode(string EmployeeCode,long Id)
        {
            if (Id > 0) return Json(true);
            var result = await _employeeRepository.checkforDuplicateBycode(EmployeeCode);
            if (result)
                return Json($"Employee Code {EmployeeCode} is already assigned");
            return Json(true);
        }

        [HttpGet]
        public async Task<JsonResult> CheckEmployeeEmail(string Email, long Id)
        {
            if (Id > 0) return Json(true);
            var result = await _employeeRepository.checkforDuplicateByEmail(Email);
            if (result)
                return Json($"{Email} is already registered");
            return Json(true);
        }

        [HttpGet]
        public async Task<JsonResult> CheckEmployeeMobile(string PhoneNumber, long Id)
        {
            if (Id > 0) return Json(true);
            var result = await _employeeRepository.checkforDuplicateByMobile(PhoneNumber);
            if (result)
                return Json($"{PhoneNumber} is already registered");
            return Json(true);
        }

        [HttpGet]
        public async Task<JsonResult> CheckEmployeeLicense(string LicenseNumber, long Id)
        {
            if (Id > 0) return Json(true);
            var result = await _employeeRepository.checkforDuplicateByLicense(LicenseNumber);
            if (result)
                return Json($"License No {LicenseNumber} is already registered");
            return Json(true);
        }
    }
}
