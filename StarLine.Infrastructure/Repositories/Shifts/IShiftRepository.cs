using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Shifts
{
    public interface IShiftRepository
    {
        Task<PagedResponse<List<ShiftGroupModel>>> GetAllShiftGroups(PaginationModel model);
        Task<ApiPostResponse<long>> AddorUpdateShift(ShiftWizardModel model);
        Task<ShiftModel> GetShiftByName(string name);
        Task<ShiftModel> GetShiftById(long id);
        Task<ShiftModel> GetShiftGeneralShift();
        Task<ShiftWizardModel> GetFullShiftDetails(long shiftId);
    }
}
