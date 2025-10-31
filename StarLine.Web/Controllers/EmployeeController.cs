using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Repositories.Employees;
using StarLine.Infrastructure.Repositories.Lists;
using StarLine.Infrastructure.Repositories.Transfer;

namespace StarLine.Web.Controllers
{
    [Authorize(Policy = "EmployeePolicy")]
    public class EmployeeController(IEmployeeRepository employeeRepository, ILookUpRepository lookUpRepository, IUserSession userSession,
        IHttpContextAccessor httpContextAccessor, ITransferRequestRepository transferRepository) : Controller
    {
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly IUserSession _userSession = userSession;
        private readonly ILookUpRepository _lookUpRepository = lookUpRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ITransferRequestRepository _transferRepository = transferRepository;

        public async Task<IActionResult> ViewProfile()
        {
            var response = await _employeeRepository.GetEmployeeDetailsById(_userSession.Current.UserId);
            string imageUrl = "/UserAvtars/default.png";
            if (!string.IsNullOrEmpty(response.UserImages))
            {
                imageUrl = "/UserAvtars/" + response.UserImages;
            }
            var request = _httpContextAccessor.HttpContext?.Request;
            string imagefilePath = $"{request.Scheme}://{request.Host}" + imageUrl;
            response.UserImages = imagefilePath;
            return View(response);
        }
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeRepository.GetTeamEmployee();
            foreach (var emp in employees)
            {
                //if (emp.Id == _userSession.Current.ReportingManager) { emp.TeamName = "Department Head"; }
                string imageUrl = "/UserAvtars/default.png";
                if (!string.IsNullOrEmpty(emp.UserImages))
                {
                    imageUrl = "/UserAvtars/" + emp.UserImages;
                }
                var request = _httpContextAccessor.HttpContext?.Request;
                string imagefilePath = $"{request.Scheme}://{request.Host}" + imageUrl;
                emp.UserImages = imagefilePath;
            }
            return View(employees);
        }
        public async Task<IActionResult> AssignTeam(long id)
        {
            ViewBag.Departmets = await _lookUpRepository.GetAllDepartment("");
            ViewBag.Designation = await _lookUpRepository.GetAllDesignation("");
            ViewBag.ReportingManager = await _lookUpRepository.GetReportingManagers("");
            ViewBag.ShiftGroups = await _lookUpRepository.GetShiftGroups("");
            var employee = await _employeeRepository.GetEmployeeById(id);
            return PartialView("_TeamAssignPartialView", employee.Data);
        }
        public async Task<IActionResult> GetShifts(long id)
        {
            var model = await _lookUpRepository.GetShift("", id);
            return Json(model);
        }
        public async Task<IActionResult> GetDepartmentTeam(long id)
        {
            var model = await _lookUpRepository.GetTeambyDepartmentId(id);
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeTeamAssign(EmployeeTeamAssignModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _employeeRepository.UpdateEmployeeTeamAssign(model);
                if (result == true)
                {
                    return Json(new { IsSuccess = result, Message = "Team assigned to employee successfully" });
                }
                return Json(new { IsSuccess = result, Message = "Team not assigned employee successfully" });
            }
            return Json(new { IsSuccess = false, Message = "Please enter required Data" });
        }
    }
}
