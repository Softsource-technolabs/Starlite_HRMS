using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;

namespace StarLine.Infrastructure.Repositories.Transfer
{
    public class TransferRequestRepository(StarLiteContext context, UserManager<IdentityUser> userManager, IMapper mapper, IUserSession userSession) : ITransferRequestRepository
    {
        private readonly StarLiteContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserSession _userSession = userSession;
        private readonly UserManager<IdentityUser> _userManager = userManager;

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

        public async Task<List<TransferRequestModel>> GetAllTransferRequests()
        {
            var employee = await _context.Employees.Include(_ => _.AspNetUser).ThenInclude(_ => _.Roles).FirstOrDefaultAsync(_ => _.Id == _userSession.Current.UserId);
            var identityUser = await _userManager.FindByIdAsync(employee.AspNetUserId);
            var IsManager = await _userManager.IsInRoleAsync(identityUser, "Department-Head");
            var IsHRManager = await _userManager.IsInRoleAsync(identityUser, "HR-Manager");
            if (IsManager || IsHRManager)
            {
                var request = await _context.TransferRequests.Include(r => r.Employee).Include(_ => _.FromDepartment).Include(_ => _.ToDepartment).ToListAsync();
                if (!IsHRManager)
                    request = request.Where(_ => _.FromDepartmentId == employee.DepartmentId || _.ToDepartmentId == employee.DepartmentId).ToList();
                return _mapper.Map<List<TransferRequestModel>>(request);
            }
            return null;
        }

        public async Task<TransferRequestModel> GetTransferRequest(long transferId)
        {
            var model = await _context.TransferRequests.Include(r => r.Employee).Include(_ => _.FromDepartment).ThenInclude(_ => _.Employees).Include(_ => _.ToDepartment).ThenInclude(_ => _.Employees).FirstOrDefaultAsync(_ => _.Id == transferId && _.IsDeleted == false && _.IsActive == true);
            if (model != null)
            {
                var data = _mapper.Map<TransferRequestModel>(model);

                var fromDeptManager = _context.Designations.Include(_ => _.Department).Where(_ => _.HierarchyLevel == 3 && _.Department.Id == model.FromDepartmentId).Select(_ => _.Employees).FirstOrDefault();
                var toDeptManager = _context.Designations.Include(_ => _.Department).Where(_ => _.HierarchyLevel == 3 && _.Department.Id == model.ToDepartmentId).Select(_ => _.Employees).FirstOrDefault();

                data.fromDepartmentManagerId = fromDeptManager.FirstOrDefault().Id;
                data.toDepartmentManagerId = toDeptManager.FirstOrDefault().Id;

                data.FromDepartmentManager = fromDeptManager.FirstOrDefault().FirstName + " " + fromDeptManager.FirstOrDefault().LastName;
                data.ToDepartmentManager = toDeptManager.FirstOrDefault().FirstName + " " + toDeptManager.FirstOrDefault().LastName;
                return data;
            }
            return null;
        }

        private async Task<long> AddTransferRequest(TransferRequestModel request)
        {
            var model = _mapper.Map<TransferRequest>(request);
            model.CurrentManagerApproval = (int)TransferStatus.Pending;
            model.ReceivingManagerApproval = (int)TransferStatus.Pending;
            model.Hrapproval = (int)TransferStatus.Pending;
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
