using AfternoonLaugh.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.Lists;
using StarLine.Infrastructure.Repositories.TrainingSessions;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class TrainingSessionController(ITrainingSessionRepository sessionRepository,ILookUpRepository lookUpRepository, IToastNotification toastNotification) : Controller
    {
        private readonly ITrainingSessionRepository _sessionRepository = sessionRepository;
        private readonly ILookUpRepository _lookUpRepository = lookUpRepository;
        private readonly IToastNotification _toastNotification = toastNotification;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetData(PaginationModel model)
        {
            var sessions = await _sessionRepository.GetAllAsync(model);
            return Json(sessions);
        }

        [HttpGet]
        public async Task<IActionResult> ManageSession(long id)
        {
            ViewBag.Trainings = await _lookUpRepository.GetAllTraining(string.Empty);
            ViewBag.Trainers = await _lookUpRepository.GetAllTrainers(string.Empty);
            ViewBag.Modes = CommonFunctions.GetEnumSelectList<TrainingMode>();
            var model = new TrainingSessionModel();
            if (id > 0)
            {
                model = await _sessionRepository.GetByIdAsync(id);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageSession(TrainingSessionModel model)
        {
            if (ModelState.IsValid)
            {
                var sessionId = await _sessionRepository.AddUpdateAsync(model);
                if (sessionId > 0)
                {
                    _toastNotification.AddSuccessToastMessage(ToastrMessages.GetMsg(ToastrModules.TrainingSession, model.Id > 0 ? ToastrMessages.Update : ToastrMessages.Add));
                    return RedirectToAction("Index");
                }
                _toastNotification.AddErrorToastMessage(ToastrMessages.GetMsg(ToastrModules.TrainingSession, ToastrMessages.Error));
            }
            ViewBag.Trainings = await _lookUpRepository.GetAllTraining(string.Empty);
            ViewBag.Trainers = await _lookUpRepository.GetAllTrainers(string.Empty);
            ViewBag.Modes = CommonFunctions.GetEnumSelectList<TrainingMode>();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeStatus(long id)
        {
            var isChanged = await _sessionRepository.ChangeStatusAsync(id);
            if (isChanged)
            {
                _toastNotification.AddSuccessToastMessage(ToastrMessages.GetMsg(ToastrModules.TrainingSession, ToastrMessages.Status));
            }
            else
            {
                _toastNotification.AddErrorToastMessage(ToastrMessages.GetMsg(ToastrModules.TrainingSession, ToastrMessages.StatusNotChanged));
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult checkEndDate(DateTime? EndDate, DateTime StartDate)
        {
            if (EndDate == null) return Json(true);

            var edate = Convert.ToDateTime(EndDate);
            if(edate < StartDate)
            {
                return Json("End date must be after start date");
            }
            return Json(true);
        }
    }
}
