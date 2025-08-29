using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.LeaveTypes
{
    public interface ILeaveTypeRepository
    {
        Task<BaseApiResponse> AddUpdateLeaveType(LeaveTypeModel leaveType);
        Task<BaseApiResponse> ToggleStatusLeaveType(long id);
        Task<BaseApiResponse> DeleteLeaveType(long id);
        Task<ApiPostResponse<LeaveTypeModel>> GetLeaveTypeById(long id);
        Task<PagedResponse<List<LeaveTypeModel>>> GetAllLeaveTypes(PaginationModel model);
        Task<ApiPostResponse<List<LeaveTypeModel>>> GetLeaveTypeList();
    }
}
