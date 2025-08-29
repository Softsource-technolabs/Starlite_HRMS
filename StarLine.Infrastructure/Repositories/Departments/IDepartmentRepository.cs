using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Departments
{
    public interface IDepartmentRepository
    {
        Task<BaseApiResponse> AddDepartment(DepartmentModel department);
        Task<BaseApiResponse> UpdateDepartment(DepartmentModel department);
        Task<BaseApiResponse> ToggleStatusDepartment(long id);
        Task<BaseApiResponse> DeleteDepartment(long id);
        Task<ApiPostResponse<DepartmentModel>> GetDepartmentById(long id);
        Task<PagedResponse<List<DepartmentModel>>> GetAllDepartments(PaginationModel model);
        Task<ApiPostResponse<List<DepartmentModel>>> GetDepartmentList();
    }
}
