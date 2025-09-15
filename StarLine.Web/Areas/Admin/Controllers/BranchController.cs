using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.Branches;

namespace StarLine.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class BranchController(IBranchRepository branchRepository, IToastNotification toastNotification) : Controller
    {
        private readonly IBranchRepository _branchRepository = branchRepository;
        private readonly IToastNotification _toastNotification = toastNotification;
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Get(PaginationModel model)
        {
            model.SortColumn = "Id";
            var result = await _branchRepository.GetAllBranchs(model);
            var jsonData = new { draw = model.draw, recordsFiltered = result.FilteredRecord, recordsTotal = result.TotalRecords, data = result.Data };
            return Json(jsonData);
        }

        [HttpGet]
        public async Task<IActionResult> ManageBranch(long id)
        {
            var model = new BranchModel();
            if (id > 0)
            {
                var result = await _branchRepository.GetBranchById(id);
                if (result.Success)
                {
                    model = result.Data;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageBranch(BranchModel model)
        {
            if (ModelState.IsValid)
            {
                var result = new BaseApiResponse();
                if (model.Id > 0)
                    result = await _branchRepository.UpdateBranch(model);
                else
                    result = await _branchRepository.AddBranch(model);

                if (result.Success)
                {
                    _toastNotification.AddSuccessToastMessage(result.Message);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _toastNotification.AddErrorToastMessage(result.Message);
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ToggleStatus(long id) => Json(await _branchRepository.ToggleStatusBranch(id));


        [HttpDelete]
        public async Task<IActionResult> Delete(long id) => Json(await _branchRepository.DeleteBranch(id));
    }
}
