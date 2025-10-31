using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;
using System.Reflection;

namespace StarLine.Infrastructure.Repositories.Trainings
{
    public class TrainingRepository(StarLiteContext context,IMapper mapper,IUserSession userSession) : ITrainingRepository
    {
        private readonly StarLiteContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserSession _userSession = userSession;
        public Task<long> AddUpdateTraining(TrainingModel model)
        {
            if(model.Id > 0)
            {
                return UpdateTraining(model);
            }
            else
            {
                return AddTraining(model);
            }
        }

        public async Task<BaseApiResponse> DeleteTraining(long id)
        {
            var training = await _context.Trainings.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if(training != null)
            {
                training.IsDeleted = true;
                training.DeletedBy = _userSession.Current.UserId;
                _context.Trainings.Update(training);
                await _context.SaveChangesAsync();
                return new BaseApiResponse { Success = true, Message = "Training deleted successfully." };
            }
            return new BaseApiResponse { Success = false, Message = "Training not found." };
        }

        public async Task<List<TrainingModel>> GetAllTraining()
        {
            var model = await _context.Trainings.Where(_ => !_.IsDeleted && _.IsActive).ToListAsync();
            if (model != null)
            {
                return _mapper.Map<List<TrainingModel>>(model);
            }
            return null;
        }

        public async Task<TrainingModel> GetTrainingById(long id)
        {
            var model = await _context.Trainings.FirstOrDefaultAsync(_ => _.Id == id && !_.IsDeleted && _.IsActive);
            if(model != null)
            {
                return _mapper.Map<TrainingModel>(model);
            }
            return null;
        }

        public async Task<BaseApiResponse> ToggleTrainingstatus(long id)
        {
            var model = await _context.Trainings.FirstOrDefaultAsync(_ => _.Id == id && !_.IsDeleted);
            if (model != null)
            {
                model.IsActive = !model.IsActive;
                model.UpdatedBy = _userSession.Current.UserId;
                _context.Trainings.Update(model);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Success = true, Message = "Training status updated successfully." };
                }
                else
                {
                    return new BaseApiResponse { Success = false, Message = "Failed to update training status." };
                }
            }
            return new BaseApiResponse { Success = false, Message = "Training not found." };
        }

        private async Task<long> AddTraining(TrainingModel model)
        {
            var training = _mapper.Map<Training>(model);
            training.CreatedBy = _userSession.Current.UserId;
            await _context.Trainings.AddAsync(training);
            return await _context.SaveChangesAsync();
        }

        private async Task<long> UpdateTraining(TrainingModel training)
        {
            var existingTraining = await _context.Trainings.FirstOrDefaultAsync(x => x.Id == training.Id);
            if(existingTraining != null)
            {
                var model = _mapper.Map(training, existingTraining);
                model.UpdatedBy = _userSession.Current.UserId;
                _context.Trainings.Update(model);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }
    }
}
