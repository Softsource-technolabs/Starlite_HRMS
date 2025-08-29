using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Holidays
{
    public interface IHolidayRepository
    {
        Task<BaseApiResponse> AddHoliday(HolidayModel holiday);
        Task<BaseApiResponse> UpdateHoliday(HolidayModel holiday);
        Task<BaseApiResponse> ToggleStatusHoliday(long id);
        Task<BaseApiResponse> DeleteHoliday(long id);
        Task<ApiPostResponse<HolidayModel>> GetHolidayById(long id);
        Task<PagedResponse<List<HolidayModel>>> GetAllHolidays(PaginationModel model);
    }
}
