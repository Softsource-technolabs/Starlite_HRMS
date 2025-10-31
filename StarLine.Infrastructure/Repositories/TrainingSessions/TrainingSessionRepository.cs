using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;
using System.Reflection;

namespace StarLine.Infrastructure.Repositories.TrainingSessions
{
    public class TrainingSessionRepository(StarLiteContext context,IUserSession userSession,IMapper mapper) : ITrainingSessionRepository
    {
        private readonly StarLiteContext _context = context;
        private readonly IUserSession _userSession = userSession;
        private readonly IMapper _mapper = mapper;
        public Task<long> AddUpdateAsync(TrainingSessionModel session)
        {
            if (session.Id > 0)
            {
                return UpdateSession(session);
            }
            else
            {
                return AddSession(session);
            }
        }

        public async Task<bool> ChangeStatusAsync(long id)
        {
            var session = await _context.TrainingSessions.FirstOrDefaultAsync(_ => _.Id == id && !_.IsDeleted && _.IsActive);
            if(session != null)
            {
                session.IsActive = !session.IsActive;
                session.ModifiedBy = _userSession.Current.UserId;
                _context.TrainingSessions.Update(session);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<PagedResponse<List<TrainingSessionModel>>> GetAllAsync(PaginationModel model)
        {
            var query = _context.TrainingSessions.Include(_ => _.Training).Where(_ => _.IsDeleted == false).AsQueryable();
            var totalRecord = query.Count();
            if (!string.IsNullOrEmpty(model.StrSearch))
            {
                query = query.Where(_ => _.Training.Name.Contains(model.StrSearch) || _.TrainingCode.Contains(model.StrSearch)
                || _.Category.Contains(model.StrSearch) || _.Location.Contains(model.StrSearch) || _.Description.Contains(model.StrSearch));
            }

            if (!string.IsNullOrWhiteSpace(model.SortOrder))
            {
                var property = typeof(Department).GetProperty(model.SortColumn ?? "Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                string columnName = property != null ? property.Name : "Id";
                bool isDescending = model.SortOrder.ToLower() == "desc";
                query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, columnName))
                    : query.OrderBy(e => EF.Property<object>(e, columnName));
            }

            var count = await query.CountAsync();
            var data = await query.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToListAsync();
            var modelData = _mapper.Map<List<TrainingSessionModel>>(data);
            return new PagedResponse<List<TrainingSessionModel>>(modelData, totalRecord, count);
        }

        public async Task<TrainingSessionModel> GetByIdAsync(long id)
        {
            var session = await _context.TrainingSessions.AsNoTracking().FirstOrDefaultAsync(_ => _.Id == id && !_.IsDeleted && _.IsActive);
            if(session != null)
            {
                var employee = await _context.Employees.FindAsync(session.TrainerId);
                var model = _mapper.Map<TrainingSessionModel>(session);
                model.TrainerName = employee != null ? $"{employee.FirstName} {employee.LastName}" : string.Empty;
                return model;
            }
            return null;
        }

        private async Task<long> AddSession(TrainingSessionModel session)
        {
            var model = _mapper.Map<TrainingSession>(session);
            model.CreatedBy = _userSession.Current.UserId;
            await _context.TrainingSessions.AddAsync(model);
            return await _context.SaveChangesAsync();
        }

        private async Task<long> UpdateSession(TrainingSessionModel session)
        {
            var existingSession = await _context.TrainingSessions.FirstOrDefaultAsync(_ => _.Id == session.Id && !_.IsDeleted && _.IsActive);
            if(existingSession != null)
            {
                var model = _mapper.Map(session,existingSession);
                model.ModifiedBy = _userSession.Current.UserId;
                _context.TrainingSessions.Update(model);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }
    }
}
