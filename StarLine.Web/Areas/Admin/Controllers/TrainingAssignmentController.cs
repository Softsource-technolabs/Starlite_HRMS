using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using StarLine.Infrastructure.Repositories.TrainingAssignments;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class TrainingAssignmentController(ITrainingAssignRepository assignRepository, IToastNotification toastNotification) : Controller
    {
        private readonly ITrainingAssignRepository _assignRepository = assignRepository;
        private readonly IToastNotification _toastNotification = toastNotification;
        public async Task<IActionResult> Index()
        {
            var model = await _assignRepository.GetAllAssignmentsAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetSessionEmployeeList(long sessionId)
        {
            var model = await _assignRepository.GetSessionAssignedEmployeeList(sessionId);
            return PartialView("_AssignedEmployeeListPartial", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetEligibleEmployees(long sessionId)
        {
            var employees = await _assignRepository.GetEligibleEmployeesAsync(sessionId);
            return PartialView("_EmployeeListPartial", employees);
        }

        [HttpGet]
        public async Task<IActionResult> AssignTraining()
        {
            ViewBag.Sessions = await _assignRepository.GetAllSessionsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignTraining(long sessionId, List<long> employeeIds)
        {
            var result = await _assignRepository.AssignEmployeesAsync(sessionId, employeeIds);
            if (result.Success)
            {
                _toastNotification.AddSuccessToastMessage(result.Message);
            }
            else
            {
                _toastNotification.AddErrorToastMessage(result.Message);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
