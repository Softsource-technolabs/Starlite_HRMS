using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;
using System.Reflection;

namespace StarLine.Infrastructure.Repositories.Leaves
{
    public class LeaveRepository(StarLiteContext context, IMapper mapper, IUserSession userSession) : ILeaveRepository
    {
        private readonly StarLiteContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserSession _userSession = userSession;
        public async Task<long> AddUpdateLeave(LeaveModel leave)
        {
            if (leave.Id > 0)
                return await UpdateLeave(leave);
            else
                return await AddLeave(leave);
        }

        public async Task<BaseApiResponse> DeleteLeave(long id)
        {
            var leave = await _context.Leaves.Where(_ => _.Id == id && _.Status == (int)LeaveStatus.Pending && _.IsDeleted == false).FirstOrDefaultAsync();
            if (leave != null)
            {
                leave.IsDeleted = !leave.IsDeleted;
                leave.DeletedBy = _userSession.Current.UserId;
                _context.Leaves.Update(leave);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return new BaseApiResponse { Success = true, Message = "Leave Deleted successfully" };
                return new BaseApiResponse { Success = false, Message = "Leave not deleted" };
            }
            return new BaseApiResponse { Success = false, Message = "Leave is not found or it's status changed" };
        }

        public async Task<PagedResponse<List<LeaveModel>>> GetAllLeaves(PaginationModel model)
        {
            var query = _context.Leaves.Include(_ => _.Employee).Include(_ => _.LeaveType).Where(_ => _.IsDeleted == false && _.EmployeeId == _userSession.Current.UserId).AsQueryable();
            var totalRecord = query.Count();
            if (!string.IsNullOrEmpty(model.StrSearch))
            {
                query = query.Where(_ => _.RejectReason.Contains(model.StrSearch) ||
                _.FromDate.ToString().Contains(model.StrSearch) || _.ToDate.ToString().Contains(model.StrSearch));
            }

            if (!string.IsNullOrWhiteSpace(model.SortOrder))
            {
                var property = typeof(Employee).GetProperty(model.SortColumn ?? "Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                string columnName = property != null ? property.Name : "Id";
                bool isDescending = model.SortOrder.Equals("desc", StringComparison.CurrentCultureIgnoreCase);
                query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, columnName))
                    : query.OrderBy(e => EF.Property<object>(e, columnName));
            }

            var count = await query.CountAsync();
            var data = await query.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToListAsync();
            var modelData = _mapper.Map<List<LeaveModel>>(data);
            return new PagedResponse<List<LeaveModel>>(modelData, totalRecord, count);
        }

        public async Task<PagedResponse<List<LeaveModel>>> GetAllLeavesForOthers(PaginationModel model)
        {
            var query = _context.Leaves.Include(_ => _.Employee).Include(_ => _.LeaveType).Where(_ => _.IsDeleted == false && _.ApproverId == _userSession.Current.UserId).AsQueryable();
            var totalRecord = query.Count();
            if (!string.IsNullOrEmpty(model.StrSearch))
            {
                query = query.Where(_ => _.RejectReason.Contains(model.StrSearch) ||
                _.FromDate.ToString().Contains(model.StrSearch) || _.ToDate.ToString().Contains(model.StrSearch));
            }

            if (!string.IsNullOrWhiteSpace(model.SortOrder))
            {
                var property = typeof(Employee).GetProperty(model.SortColumn ?? "Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                string columnName = property != null ? property.Name : "Id";
                bool isDescending = model.SortOrder.Equals("desc", StringComparison.CurrentCultureIgnoreCase);
                query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, columnName))
                    : query.OrderBy(e => EF.Property<object>(e, columnName));
            }

            var count = await query.CountAsync();
            var data = await query.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToListAsync();
            var modelData = _mapper.Map<List<LeaveModel>>(data);
            return new PagedResponse<List<LeaveModel>>(modelData, totalRecord, count);
        }

        public async Task<LeaveModel> GetLeaveById(long id)
        {
            var leave = await _context.Leaves.Include(_ => _.Employee).Include(_ => _.LeaveType).Where(_ => _.Id == id && _.Status == (int)LeaveStatus.Pending && _.IsDeleted == false).FirstOrDefaultAsync();
            if (leave != null)
            {
                return _mapper.Map<LeaveModel>(leave);
            }
            return null;
        }

        public async Task<BaseApiResponse> UpdateLeaveStatus(long id, LeaveStatus status)
        {
            var leave = await _context.Leaves.Where(_ => _.Id == id && _.IsDeleted == false).FirstOrDefaultAsync();
            if (leave != null)
            {
                leave.Status = (int)status;
                leave.UpdatedBy = _userSession.Current.UserId;
                _context.Leaves.Update(leave);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return new BaseApiResponse { Success = true, Message = "Leave status updated" };
                else
                    return new BaseApiResponse { Success = false, Message = "Leave status not updated" };
            }
            return new BaseApiResponse { Success = false, Message = "Leave not found" };
        }

        private async Task<long> AddLeave(LeaveModel leave)
        {
            var model = _mapper.Map<Leaf>(leave);
            model.EmployeeId = _userSession.Current.UserId;
            model.ApproverId = _userSession.Current.ReportingManager;
            await _context.Leaves.AddAsync(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        private async Task<long> UpdateLeave(LeaveModel model)
        {
            var existingLeave = await _context.Leaves.FirstOrDefaultAsync(_ => _.Id == model.Id && _.IsDeleted == false);
            if (existingLeave != null)
            {
                if (existingLeave.LeaveTypeId == (int)LeaveStatus.Pending)
                {
                    var leave = _mapper.Map(model, existingLeave);
                    leave.UpdatedBy = _userSession.Current.UserId;

                    _context.Leaves.Update(leave);
                    return await _context.SaveChangesAsync(); // If record updated then 1
                }
                return 0; // If record found with status change from pending then return 0
            }
            return -1; // If leave not found then return -1
        }
    }
}
