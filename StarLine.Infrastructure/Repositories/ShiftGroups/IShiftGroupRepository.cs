using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Shifts
{
    public interface IShiftGroupRepository
    {
        Task<List<ShiftGroupModel>> GetShiftGroupList();
        Task<ApiPostResponse<List<ShiftGroupModel>>> GetAllShiftGroups();
        Task<ApiPostResponse<long>> AddShiftGroup(ShiftGroupModel model);
        Task<ApiPostResponse<ShiftGroupModel>> GetShiftGroup(long id);
        Task<ApiPostResponse<long>> UpdateShiftGroup(long id, ShiftGroupModel model);
        Task<BaseApiResponse> ToggleStatus(long id);
        Task<BaseApiResponse> DeleteShiftGroup(long id);
    }
}
