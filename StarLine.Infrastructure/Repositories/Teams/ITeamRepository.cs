using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Teams
{
    public interface ITeamRepository
    {
        Task<BaseApiResponse> AddTeam(TeamModel team);
        Task<BaseApiResponse> UpdateTeam(TeamModel team);
        Task<BaseApiResponse> ToggleStatusTeam(long id);
        Task<BaseApiResponse> DeleteTeam(long id);
        Task<ApiPostResponse<TeamModel>> GetTeamById(long id);
        Task<PagedResponse<List<TeamModel>>> GetAllTeams(PaginationModel model);
    }
}
