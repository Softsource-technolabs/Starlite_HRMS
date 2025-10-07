using AfternoonLaugh.Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.Leaves;
using StarLine.Infrastructure.Repositories.LeaveTypes;

namespace StarLine.Web.Controllers
{
    [Authorize(Policy = "EmployeePolicy")]
    public class LeaveController(ILeaveRepository leaveRepository, IToastNotification toastNotification, ILeaveTypeRepository leaveTypeRepository) : Controller
    {
        private readonly ILeaveRepository _leaveRepository = leaveRepository;
        private readonly IToastNotification _toastNotification = toastNotification;
        private readonly ILeaveTypeRepository _leaveTypeRepository = leaveTypeRepository;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Get(PaginationModel model)
        {
            var result = await _leaveRepository.GetAllLeaves(model);
            return Json(new
            {
                model.draw,
                result.recordsFiltered,
                result.recordsTotal,
                data = result.Data
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetEmployeeLeaves(PaginationModel model)
        {
            var leaves = await _leaveRepository.GetAllLeavesForOthers(model);
            return Json(leaves);
        }

        [HttpGet]
        public async Task<IActionResult> ManageLeave(long id = 0)
        {
            ViewBag.LeaveStatus = CommonFunctions.GetEnumSelectList<LeaveStatus>();
            ViewBag.LeaveDurations = CommonFunctions.GetEnumSelectList<LeaveDuration>();
            var leavetypes = await _leaveTypeRepository.GetLeaveTypeList();
            var leavetypeList = leavetypes.Data;
            leavetypeList.Insert(0, new LeaveTypeModel
            {
                Id = 0,
                TypeName = "Select Leave Type"
            });
            ViewBag.leaveTypes = leavetypeList.Select(_ => new SelectListItem
            {
                Value = _.Id.ToString(),
                Text = _.TypeName,
            });
            var model = new LeaveModel();
            if (id > 0)
            {
                model = await _leaveRepository.GetLeaveById(id);
            }
            return PartialView("_ManageLeavePartialView", model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageLeave(LeaveModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _leaveRepository.AddUpdateLeave(model);
                if (result > 0)
                {
                    _toastNotification.AddSuccessToastMessage(ToastrMessages.GetMsg(ToastrModules.Leave, ToastrMessages.Add));
                    return RedirectToAction(nameof(Index));
                }
                if (result == 0)
                {
                    _toastNotification.AddErrorToastMessage(ToastrMessages.GetMsg(ToastrModules.Leave, "Leave status already changed"));
                }
                if (result == -1)
                {
                    _toastNotification.AddErrorToastMessage(ToastrMessages.GetMsg(ToastrModules.Leave, ToastrMessages.NotFound));
                }
            }
            return PartialView("_ManageLeavePartialView", model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeStatus(long id, LeaveStatus status)
        {
            if (id > 0)
            {
                var result = await _leaveRepository.UpdateLeaveStatus(id, status);
                return Json(new { success = result.Success, message = result.Message });
            }
            return Json(false);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(long id)
        {
            if (id > 0)
            {
                var result = await _leaveRepository.DeleteLeave(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            return Json(false);
        }

        [HttpGet]
        public async Task<IActionResult> LeaveDetails(long id)
        {
            var model = await _leaveRepository.GetLeaveById(id);
            return PartialView("_LeaveApprovalPartialView", model);
        }

        [HttpPost]
        public async Task<IActionResult> LeaveDetails(LeaveModel model)
        {
            var result = await _leaveRepository.AddUpdateLeave(model);
            if (result > 0)
            {
                return Json(new { success = true, Message = $"Leave Request {model.statusName}" });
            }
            return Json(new { success = false, Message = "Error while processing leave request" });
        }
    }
}
