using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.TrainingSessions
{
    public interface ITrainingSessionRepository
    {
        Task<PagedResponse<List<TrainingSessionModel>>> GetAllAsync(PaginationModel model);
        Task<TrainingSessionModel> GetByIdAsync(long id);
        Task<long> AddUpdateAsync(TrainingSessionModel session);
        Task<bool> ChangeStatusAsync(long id);
    }
}
