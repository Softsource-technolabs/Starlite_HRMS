using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Transfer
{
    public interface ITransferRequestRepository
    {
        Task<long> AddUpdateTransferRequest(TransferRequestModel model);
        Task<bool> DeleteTransferRequest(long transferId);
        Task<TransferRequestModel> GetTransferRequest(long transferId);
        Task<PagedResponse<List<TransferRequestModel>>> GetAllTransferRequests(PaginationModel model);
    }
}
