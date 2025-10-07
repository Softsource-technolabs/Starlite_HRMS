using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Leaves
{
    public interface ILeaveRepository
    {
        Task<long> AddUpdateLeave(LeaveModel leave);
        Task<BaseApiResponse> UpdateLeaveStatus(long id, LeaveStatus status);
        Task<BaseApiResponse> DeleteLeave(long id);
        Task<LeaveModel> GetLeaveById(long id);
        Task<PagedResponse<List<LeaveModel>>> GetAllLeaves(PaginationModel model);
        Task<PagedResponse<List<LeaveModel>>> GetAllLeavesForOthers(PaginationModel model);
    }
}
