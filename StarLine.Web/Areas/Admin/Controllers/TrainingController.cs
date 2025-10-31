using AfternoonLaugh.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.Trainings;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class TrainingController(ITrainingRepository trainingRepository, IToastNotification toastNotification) : Controller
    {
        private readonly ITrainingRepository _trainingRepository = trainingRepository;
        private readonly IToastNotification _toastNotification = toastNotification;
        public async Task<IActionResult> Index()
        {
            var model = await _trainingRepository.GetAllTraining();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ManageTraining(long id)
        {
            var model = new TrainingModel();
            if (id > 0)
            {
                model = await _trainingRepository.GetTrainingById(id);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageTraining(TrainingModel model)
        {
            if (ModelState.IsValid)
            {
                var trainingId = await _trainingRepository.AddUpdateTraining(model);
                if (trainingId > 0)
                {
                    _toastNotification.AddSuccessToastMessage(ToastrMessages.GetMsg(ToastrModules.Training, model.Id > 0 ? ToastrMessages.Update : ToastrMessages.Add));
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ToggleTrainingStatus(long id)
        {
            var result = await _trainingRepository.ToggleTrainingstatus(id);
            if (result.Success)
            {
                _toastNotification.AddSuccessToastMessage(ToastrMessages.GetMsg(ToastrModules.Training, ToastrMessages.Status));
                return Json(true);
            }
            else
            {
                _toastNotification.AddErrorToastMessage(ToastrMessages.GetMsg(ToastrModules.Training, ToastrMessages.NotFound));
                return Json(false);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTraining(long id)
        {
            var result = await _trainingRepository.DeleteTraining(id);
            if (result.Success)
            {
                _toastNotification.AddSuccessToastMessage(ToastrMessages.GetMsg(ToastrModules.Training, ToastrMessages.Delete));
                return Json(true);
            }
            else
            {
                _toastNotification.AddErrorToastMessage(ToastrMessages.GetMsg(ToastrModules.Training, ToastrMessages.NotDelete));
                return Json(false);
            }
        }
    }
}
