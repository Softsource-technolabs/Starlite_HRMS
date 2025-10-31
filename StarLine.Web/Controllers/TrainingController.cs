using Microsoft.AspNetCore.Mvc;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Repositories.TrainingAssignments;

namespace StarLine.Web.Controllers
{
    public class TrainingController(ITrainingAssignRepository assignRepository, IUserSession userSession) : Controller
    {
        private readonly IUserSession _userSession = userSession;
        private readonly ITrainingAssignRepository _assignRepository = assignRepository;
        public async Task<IActionResult> Index()
        {
            var trainerid = _userSession.Current.UserId;
            var model = await _assignRepository.GetAllSessionsByTrainerAsync(trainerid);
            return View(model);
        }

        public async Task<IActionResult> TrainingCompletion(long sessionId)
        {
            var employees = await _assignRepository.GetSessionAssignedEmployeeList(sessionId);
            return View(employees);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCompletion([FromBody] List<TrainingSessionCompletionModel> updates)
        {
            var success = await _assignRepository.UpdateCompletionStatusAsync(updates);
            return Json(new { success });
        }
    }
}
