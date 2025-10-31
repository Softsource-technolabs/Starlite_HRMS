using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Trainings
{
    public interface ITrainingRepository
    {
        Task<long> AddUpdateTraining(TrainingModel model); 
        Task<TrainingModel> GetTrainingById(long id);
        Task<List<TrainingModel>> GetAllTraining();
        Task<BaseApiResponse> ToggleTrainingstatus(long id);
        Task<BaseApiResponse> DeleteTraining(long id);
    }
}
