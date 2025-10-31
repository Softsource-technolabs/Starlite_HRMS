using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.Transfer;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class TransferController(ITransferRequestRepository requestRepository,IToastNotification toastNotification) : Controller
    {
        private readonly ITransferRequestRepository _requestRepository = requestRepository;
        private readonly IToastNotification _notification = toastNotification;
        public async Task<IActionResult> Index()
        {
            var model = await _requestRepository.GetAllTransferRequests();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> TransferRequest(long id)
        {
            var model = await _requestRepository.GetTransferRequest(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TransferRequest(TransferRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _requestRepository.AddUpdateTransferRequest(model);
                if (result > 0)
                {
                    _notification.AddSuccessToastMessage("Transfer Request updated Successfully");
                    return RedirectToAction(nameof(Index));
                }
                _notification.AddSuccessToastMessage("Error while updating Transfer Request");
            }
            return View(model);
        }
    }
}
