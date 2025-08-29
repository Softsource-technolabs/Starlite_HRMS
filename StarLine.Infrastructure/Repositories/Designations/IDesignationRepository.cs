using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Designations
{
    public interface IDesignationRepository
    {
        Task<BaseApiResponse> AddDesignation(DesignationModel designation);
        Task<BaseApiResponse> UpdateDesignation(DesignationModel designation);
        Task<BaseApiResponse> ToggleStatusDesignation(long id);
        Task<BaseApiResponse> DeleteDesignation(long id);
        Task<ApiPostResponse<DesignationModel>> GetDesignationById(long id);
        Task<PagedResponse<List<DesignationModel>>> GetAllDesignations(PaginationModel model);
        Task<ApiPostResponse<List<DesignationModel>>> GetDesignationList();
        Task<List<DesignationModel>> GetDesignationByDepartment(long departmentId);
    }
}
