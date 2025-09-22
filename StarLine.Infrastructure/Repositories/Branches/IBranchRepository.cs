using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Branches
{
    public interface IBranchRepository
    {
        Task<long> AddUpdateBranch(BranchModel branch);
        Task<BaseApiResponse> ToggleStatusBranch(long id);
        Task<BaseApiResponse> DeleteBranch(long id);
        Task<BranchModel> GetBranchById(long id);
        Task<PagedResponse<List<BranchModel>>> GetAllBranchs(PaginationModel model);
        Task<List<BranchModel>> GetBranchList();
    }
}
