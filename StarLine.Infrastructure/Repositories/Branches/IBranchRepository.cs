using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Branches
{
    public interface IBranchRepository
    {
        Task<BaseApiResponse> AddBranch(BranchModel branch);
        Task<BaseApiResponse> UpdateBranch(BranchModel branch);
        Task<BaseApiResponse> ToggleStatusBranch(long id);
        Task<BaseApiResponse> DeleteBranch(long id);
        Task<ApiPostResponse<BranchModel>> GetBranchById(long id);
        Task<PagedResponse<List<BranchModel>>> GetAllBranchs(PaginationModel model);
        Task<ApiPostResponse<List<BranchModel>>> GetBranchList();
    }
}
