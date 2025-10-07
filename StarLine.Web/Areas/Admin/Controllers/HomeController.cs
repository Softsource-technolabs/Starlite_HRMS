using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Repositories.Attendace;
using StarLine.Infrastructure.Repositories.Employees;
using StarLine.Infrastructure.Repositories.Holidays;
using StarLine.Infrastructure.Repositories.Shifts;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class HomeController(IHolidayRepository holidayRepository, IEmployeeRepository employeeRepository, IShiftRepository shiftRepository, IUserSession userSession, IAttendanceRepository attendanceRepository) : Controller
    {
        private readonly IHolidayRepository _holidayRepository = holidayRepository;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly IShiftRepository _shiftRepository = shiftRepository;
        private readonly IUserSession _userSession = userSession;
        private readonly IAttendanceRepository _attendanceRepository = attendanceRepository;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetCurrentYearHoliday(PaginationModel model)
        {
            var result = await _holidayRepository.GetHolidaysforCurrentYear(model);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyAttendance()
        {
            var currentUser = _userSession.Current.UserId;
            var result = await _attendanceRepository.GetAttendanceList(currentUser);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendace()
        {
            var currentUser = _userSession.Current.UserId;
            var employeeDetails = await _employeeRepository.GetEmployeeById(currentUser);
            if (employeeDetails != null)
            {
                var attendances = await _attendanceRepository.GetAttendanceByEmpId(currentUser);
                if (attendances != null)
                {
                    var employeeattendace = new
                    {
                        success = true,
                        shift = new
                        {
                            shiftName = employeeDetails.Data.ShiftNameandTiming,
                            isClockedIn = attendances.InTime != null ? true : false,
                            isclockedOut = attendances.OutTime != null ? true : false,
                            Status = attendances.StatusName
                        }
                    };
                    return Json(employeeattendace);
                }
                else
                {
                    var data = new
                    {
                        success = false,
                        shift = ""
                    };
                    return Json(data);
                }
            }
            return Json(new { success = false, shift = "" });
        }

        [HttpGet]
        public async Task<IActionResult> MarkclockIn()
        {
            long empId = _userSession.Current.UserId;
            var employeeDetails = await _employeeRepository.GetEmployeeById(empId);
            var shift = await _shiftRepository.GetShiftById(employeeDetails.Data.shiftId);

            var model = new AttendanceModel
            {
                EmployeeId = empId,
                Attendacedate = DateOnly.FromDateTime(DateTime.Now),
                InTime = TimeOnly.FromDateTime(DateTime.Now),
                Status = AttendaceStatus.Present,
            };
            var result = await _attendanceRepository.AddUpdateAttendace(model);
            if (result > 0)
                return Json(true);
            else
                return Json(false);
        }

        [HttpGet]
        public async Task<IActionResult> MarkclockOut()
        {
            long empId = _userSession.Current.UserId;
            var employeeDetails = await _employeeRepository.GetEmployeeById(empId);
            var attendance = await _attendanceRepository.GetAttendanceByEmpId(empId);
            if (attendance != null)
            {
                attendance.OutTime = TimeOnly.FromDateTime(DateTime.Now);
                attendance.Status = AttendaceStatus.Away;
                var result = await _attendanceRepository.AddUpdateAttendace(attendance);
                if (result > 0)
                    return Json(true);
                else
                    return Json(false);
            }
            return Json(false);
        }
    }
}
