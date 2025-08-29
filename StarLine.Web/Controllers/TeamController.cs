using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.Departments;
using StarLine.Infrastructure.Repositories.Teams;

namespace StarLine.Web.Controllers
{
    [Authorize("Department-Head")]
    public class TeamController : Controller
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IToastNotification _toastNotification;
        public TeamController(ITeamRepository teamRepository, IDepartmentRepository departmentRepository, IToastNotification toastNotification)
        {
            _teamRepository = teamRepository;
            _departmentRepository = departmentRepository;
            _toastNotification = toastNotification;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Get(PaginationModel model)
        {
            model.SortColumn = "Id";
            var result = await _teamRepository.GetAllTeams(model);
            var jsonData = new { draw = model.draw, recordsFiltered = result.FilteredRecord, recordsTotal = result.TotalRecords, data = result.Data };
            return Json(jsonData);
        }

        [HttpGet]
        public async Task<IActionResult> ManageTeam(long? id)
        {
            var model = new TeamModel();
            var employees = await _teamRepository.GetTeamLead();
            ViewBag.TeamLeads = employees.Select(_ => new SelectListItem
            {
                Text = _.FirstName + " " + _.LastName + $" ({_.AspNetUser.Roles.FirstOrDefault().Name})",
                Value = _.Id.ToString()
            }).ToList();
            var departments = await _departmentRepository.GetDepartmentList();
            ViewBag.TeamTypes = CommonFunctions.GetEnumSelectList<TeamType>();
            ViewBag.departments = departments.Data.Select(_ => new SelectListItem
            {
                Text = _.DepartmentName,
                Value = _.Id.ToString()
            }).ToList();
            if (id != null && id > 0)
            {
                var result = await _teamRepository.GetTeamById((long)id);
                model = result.Data;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageTeam(TeamModel model)
        {
            if (ModelState.IsValid)
            {
                var result = new BaseApiResponse();
                if (model.Id > 0)
                    result = await _teamRepository.UpdateTeam(model);
                else
                    result = await _teamRepository.AddTeam(model);

                if (result.Success)
                    _toastNotification.AddSuccessToastMessage(result.Message);
                else
                    _toastNotification.AddErrorToastMessage(result.Message);

                return RedirectToAction(nameof(Index));
            }

            var employees = await _teamRepository.GetTeamLead();
            ViewBag.TeamLeads = employees.Select(_ => new SelectListItem
            {
                Text = _.FirstName + " " + _.LastName + $" ({_.AspNetUser.Roles.FirstOrDefault().Name})",
                Value = _.Id.ToString()
            }).ToList();
            var departments = await _departmentRepository.GetDepartmentList();
            ViewBag.TeamTypes = CommonFunctions.GetEnumSelectList<TeamType>();
            ViewBag.departments = departments.Data.Select(_ => new SelectListItem
            {
                Text = _.DepartmentName,
                Value = _.Id.ToString()
            }).ToList();
            return View(model);
        }

        [HttpGet]
        public Task<IActionResult> ViewTeamMembers(long teamId)
        {

        }

        [HttpGet]
        public async Task<IActionResult> ToggleActivation(long id)
        {
            return Json(await _teamRepository.ToggleStatusTeam(id));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(long id)
        {
            return Json(await _teamRepository.DeleteTeam(id));
        }
    }
}
