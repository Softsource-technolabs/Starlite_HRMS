using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Notices
{
    public interface INoticeRepository
    {
        Task<BaseApiResponse> AddUpdateNotice(NoticeModel notice);
        Task<BaseApiResponse> ToggleStatusNotice(long id);
        Task<BaseApiResponse> DeleteNotice(long id);
        Task<ApiPostResponse<NoticeModel>> GetNoticeById(long id);
        Task<PagedResponse<List<NoticeModel>>> GetAllNotices(PaginationModel model);
    }
}
