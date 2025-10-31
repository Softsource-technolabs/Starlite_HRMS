using Microsoft.AspNetCore.Mvc.Rendering;

namespace StarLine.Infrastructure.Repositories.Lists
{
    public interface ILookUpRepository
    {
        Task<List<SelectListItem>> GetAllDepartment(string searchText);
        Task<List<SelectListItem>> GetAllDesignation(string searchText);
        Task<List<SelectListItem>> GetReportingManagers(string searchText);
        Task<List<SelectListItem>> GetShiftGroups(string searchText);
        Task<List<SelectListItem>> GetShift(string searchText, long shiftgroupId);
        Task<List<SelectListItem>> GetAllRoles(string searchText);
        Task<List<SelectListItem>> GetAllUsers(string searchText);
        Task<List<SelectListItem>> GetTeambyDepartmentId(long departmentId);
        Task<List<SelectListItem>> GetTeamMembers(long departmentId,long currentUserId);
        Task<List<SelectListItem>> GetAllTraining(string searchText);
        Task<List<SelectListItem>> GetAllTrainers(string searchText);
    }
}
