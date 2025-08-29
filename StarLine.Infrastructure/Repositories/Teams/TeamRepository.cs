using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;
using System.Reflection;

namespace StarLine.Infrastructure.Repositories.Teams
{
    public class TeamRepository : ITeamRepository
    {
        private readonly StarLiteContext _context;
        private readonly IMapper _mapper;
        private readonly IUserSession _userSession;
        public TeamRepository(StarLiteContext context, IMapper mapper, IUserSession userSession)
        {
            _context = context;
            _mapper = mapper;
            _userSession = userSession;
        }
        public async Task<BaseApiResponse> AddTeam(TeamModel team)
        {
            var model = _mapper.Map<Team>(team);
            model.CreatedBy = _userSession.Current.UserId;
            await _context.Teams.AddAsync(model);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var teamMember = new TeamMember
                {
                    TeamId = model.Id,
                    CreatedBy = _userSession.Current.UserId,
                    IsDeleted = false
                };
                await _context.TeamMembers.AddAsync(teamMember);
                var memberResult = await _context.SaveChangesAsync();
                if (result > 0 && memberResult > 0)
                    return new BaseApiResponse { Success = true, Message = "Team added successfully" };
            }
            return new BaseApiResponse { Success = true, Message = "Team not added" };
        }

        public async Task<BaseApiResponse> DeleteTeam(long id)
        {
            var model = await _context.Teams.FirstOrDefaultAsync(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false);
            if (model != null)
            {
                model.IsDeleted = true;
                model.DeletedBy = _userSession.Current.UserId;
                _context.Teams.Update(model);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Success = true, Message = "Team deleted successfully" };
                }
                return new BaseApiResponse { Success = false, Message = "Team not deleted" };
            }
            return new BaseApiResponse { Success = false, Message = "Team not found" };
        }

        public async Task<PagedResponse<List<TeamModel>>> GetAllTeams(PaginationModel model)
        {
            var query = _context.Teams.Include(_ => _.Department).Include(_ => _.TeamMembers).ThenInclude(_ => _.Employee).Where(_ => _.IsDeleted == false).AsQueryable();
            var totalRecord = query.Count();
            if (!string.IsNullOrEmpty(model.StrSearch))
            {
                query = query.Where(_ => _.Name.Contains(model.StrSearch) || _.Description.Contains(model.StrSearch)
                || _.Department.DepartmentName.Contains(model.StrSearch));
            }

            if (!string.IsNullOrWhiteSpace(model.SortOrder))
            {
                var property = typeof(Team).GetProperty(model.SortColumn ?? "Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                string columnName = property != null ? property.Name : "Id";
                bool isDescending = model.SortOrder.ToLower() == "desc";
                query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, columnName))
                    : query.OrderBy(e => EF.Property<object>(e, columnName));
            }
            var count = await query.CountAsync();
            var data = await query.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).Select(_ => new TeamModel
            {
                Id = _.Id,
                DepartmentId = _.DepartmentId,
                Description = _.Description,
                IsActive = _.IsActive,
                Name = _.Name,
                TeamType = _.TeamType,
                DepartmentName = _.Department.DepartmentName
            }).ToListAsync();
            return new PagedResponse<List<TeamModel>>(data, model.PageNumber, model.PageSize, totalRecord, count);
        }

        public async Task<ApiPostResponse<TeamModel>> GetTeamById(long id)
        {
            var team = await _context.Teams.Include(_ => _.Department).Include(_ => _.TeamMembers).FirstOrDefaultAsync(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false);
            if (team != null)
            {
                var model = _mapper.Map<TeamModel>(team);
                return new ApiPostResponse<TeamModel> { Data = model, Success = true, Message = "Team found" };
            }
            return new ApiPostResponse<TeamModel> { Success = false, Message = "Team not found" };
        }

        public async Task<BaseApiResponse> ToggleStatusTeam(long id)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(_ => _.Id == id && _.IsDeleted == false);
            if (team != null)
            {
                team.IsActive = !team.IsActive;
                _context.Teams.Update(team);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Success = true, Message = "Team status changed" };
                }
                return new BaseApiResponse { Success = false, Message = "Team status not changed" };
            }
            return new ApiPostResponse<TeamModel> { Success = false, Message = "Team not found" };
        }

        public async Task<BaseApiResponse> UpdateTeam(TeamModel team)
        {
            var model = await _context.Teams.FirstOrDefaultAsync(_ => _.Id == team.Id && _.IsActive == true && _.IsDeleted == false);
            if (model != null)
            {
                var newTeam = _mapper.Map(team, model);
                newTeam.UpdatedBy = model.UpdatedBy;
                _context.Teams.Update(newTeam);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Success = true, Message = "Team updated successfully" };
                }
                return new BaseApiResponse { Success = false, Message = "Team not updated" };
            }
            return new BaseApiResponse { Success = false, Message = "Team not found" };
        }
    }
}
