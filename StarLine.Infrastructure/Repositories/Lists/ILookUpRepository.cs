using Microsoft.AspNetCore.Mvc.Rendering;

namespace StarLine.Infrastructure.Repositories.Lists
{
    public interface ILookUpRepository
    {
        Task<List<SelectListItem>> GetAllDepartment(string searchText);
        Task<List<SelectListItem>> GetAllRoles(string searchText);
        Task<List<SelectListItem>> GetAllUsers(string searchText);
    }
}
