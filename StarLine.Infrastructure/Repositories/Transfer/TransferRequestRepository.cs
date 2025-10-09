using AutoMapper;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;
using System.Reflection;

namespace StarLine.Infrastructure.Repositories.Transfer
{
    public class TransferRequestRepository(StarLiteContext context, IMapper mapper,IUserSession userSession) : ITransferRequestRepository
    {
        private readonly StarLiteContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserSession _userSession = userSession;

        public Task<long> AddUpdateTransferRequest(TransferRequestModel model)
        {
            if (model.Id > 0)
            {
                return UpdateTransferRequest(model);
            }
            else
            {
                return AddTransferRequest(model);
            }
        }

        public async Task<bool> DeleteTransferRequest(long transferId)
        {
            var model = await _context.TransferRequests.FirstOrDefaultAsync(_ => _.Id == transferId && _.IsDeleted == false && _.IsActive == true);
            if (model != null)
            {
                model.IsDeleted = true;
                _context.TransferRequests.Update(model);
                var result = await _context.SaveChangesAsync();
                return result > 0 ? true : false;
            }
            return false;
        }

        public async Task<PagedResponse<List<TransferRequestModel>>> GetAllTransferRequests(PaginationModel model)
        {
            var managerId = _userSession.Current.UserId;
            var query = _context.TransferRequests.Include(r => r.Employee).Include(_ => _.FromDepartment).Include(_ => _.ToDepartment)
                .Where(r => r.Employee.ReportingManagerId == managerId && r.Status == (int)TransferStatus.Pending).AsQueryable();
            
            int totalCount = await query.CountAsync();
            
            if (!string.IsNullOrEmpty(model.StrSearch))
            {
                query = query.Where(r => r.Employee.FirstName.Contains(model.StrSearch) || r.Employee.LastName.Contains(model.StrSearch)
                || r.Employee.EmployeeCode.Contains(model.StrSearch) || r.FromDepartment.DepartmentName.Contains(model.StrSearch)
                || r.ToDepartment.DepartmentName.Contains(model.StrSearch));
            }

            if (!string.IsNullOrWhiteSpace(model.SortOrder))
            {
                var property = typeof(TransferRequest).GetProperty(model.SortColumn ?? "Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                string columnName = property != null ? property.Name : "Id";
                bool isDescending = model.SortOrder.ToLower() == "desc";
                query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, columnName))
                    : query.OrderBy(e => EF.Property<object>(e, columnName));
            }
            var count = await query.CountAsync();
            var data = await query.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).Select(_ => new TransferRequestModel
            {
                Id = _.Id,
                EmployeeId = _.EmployeeId,
                CurrentManagerApproval = _.CurrentManagerApproval,
                EffectiveDate = _.EffectiveDate,
                EmployeeName = _.Employee.FirstName + " " + _.Employee.LastName,
                FromDepartmentId = _.FromDepartmentId,
                FromDepartmentName = _.FromDepartment.DepartmentName,
                Hrapproval = _.Hrapproval,
                IsActive = _.IsActive,
                IsDeleted = _.IsDeleted,
                Reason = _.Reason,
                ReceivingManagerApproval = _.ReceivingManagerApproval,
                Status = _.Status,
                ToDepartmentId = _.ToDepartmentId,
                ToDepartmentName = _.ToDepartment.DepartmentName,
            }).ToListAsync();
            return new PagedResponse<List<TransferRequestModel>>(data, totalCount, count);
        }

        public async Task<TransferRequestModel> GetTransferRequest(long transferId)
        {
            var model = await _context.TransferRequests.FirstOrDefaultAsync(_ => _.Id == transferId && _.IsDeleted == false && _.IsActive == true);
            if (model != null)
            {
                return _mapper.Map<TransferRequestModel>(model);
            }
            return null;
        }

        private async Task<long> AddTransferRequest(TransferRequestModel request)
        {
            var model = _mapper.Map<TransferRequest>(request);
            model.IsActive = true;
            await _context.TransferRequests.AddAsync(model);
            var result = await _context.SaveChangesAsync();
            return result > 0 ? model.Id : 0;
        }

        private async Task<long> UpdateTransferRequest(TransferRequestModel request)
        {
            var existingRequest = await _context.TransferRequests.FirstOrDefaultAsync(_ => _.Id == request.Id && _.IsDeleted == false && _.IsActive == true);
            if (existingRequest != null)
            {
                var model = _mapper.Map(request, existingRequest);
                _context.TransferRequests.Update(model);
                var result = await _context.SaveChangesAsync();
                return result;
            }
            return 0;
        }
    }
}
