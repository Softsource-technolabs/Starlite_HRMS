using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;
using System.Reflection;

namespace StarLine.Infrastructure.Repositories.LeaveTypes
{
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly StarLiteContext _context;
        private IMapper _mapper;
        private readonly IUserSession _userSession;

        public LeaveTypeRepository(StarLiteContext context, IMapper mapper, IUserSession userSession)
        {
            _context = context;
            _mapper = mapper;
            _userSession = userSession;
        }
        public async Task<BaseApiResponse> AddUpdateLeaveType(LeaveTypeModel leaveType)
        {
            int resultvalue = 0;
            if (leaveType.Id == 0)
            {
                var model = _mapper.Map<LeaveType>(leaveType);
                await _context.LeaveTypes.AddAsync(model);
                resultvalue = await _context.SaveChangesAsync();
            }
            else
            {
                var existingLeave = await _context.LeaveTypes.FindAsync(leaveType.Id);
                _context.Entry(existingLeave).CurrentValues.SetValues(leaveType);
                resultvalue = await _context.SaveChangesAsync();
            }
            if (resultvalue > 0)
                return new BaseApiResponse { Message = leaveType.Id > 0 ? "Leave Updated Successfully" : "Leave Added Successfully", Success = true };
            else
                return new BaseApiResponse { Message = "Leave Updated Successfully", Success = false };
        }

        public async Task<BaseApiResponse> DeleteLeaveType(long id)
        {
            var model = await _context.LeaveTypes.FirstOrDefaultAsync(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false);
            if (model != null)
            {
                model.IsDeleted = true;
                model.DeletedBy = _userSession.Current.UserId;
                _context.LeaveTypes.Update(model);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return new BaseApiResponse { Message = "Leave Type is Deleted", Success = true };
                else
                    return new BaseApiResponse { Message = "Leave Type is not deleted", Success = false };
            }
            return new BaseApiResponse { Message = "Leave Type not found", Success = false };
        }

        public async Task<PagedResponse<List<LeaveTypeModel>>> GetAllLeaveTypes(PaginationModel model)
        {
            var query = _context.LeaveTypes.Where(_ => _.IsDeleted == false).AsQueryable();
            var totalRecord = query.Count();
            if (!string.IsNullOrEmpty(model.StrSearch))
            {
                query = query.Where(_ => _.TypeName.Contains(model.StrSearch) || _.Description.Contains(model.StrSearch));
            }

            if (!string.IsNullOrWhiteSpace(model.SortOrder))
            {
                var property = typeof(LeaveType).GetProperty(model.SortColumn ?? "Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                string columnName = property != null ? property.Name : "Id";
                bool isDescending = model.SortOrder.ToLower() == "desc";
                query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, columnName))
                    : query.OrderBy(e => EF.Property<object>(e, columnName));
            }

            var count = await query.CountAsync();
            var data = await query.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToListAsync();
            var modelData = _mapper.Map<List<LeaveTypeModel>>(data);
            return new PagedResponse<List<LeaveTypeModel>>(modelData, model.PageNumber, model.PageSize, totalRecord, count);
        }

        public async Task<ApiPostResponse<LeaveTypeModel>> GetLeaveTypeById(long id)
        {
            var model = await _context.LeaveTypes.FirstOrDefaultAsync(_ => _.IsActive == true && _.IsDeleted == false);
            if (model != null)
            {
                var leaveTypes = _mapper.Map<LeaveTypeModel>(model);
                return new ApiPostResponse<LeaveTypeModel> { Data = leaveTypes, Message = "Leave Type List found", Success = true };
            }
            return new ApiPostResponse<LeaveTypeModel> { Message = "Leave Type List not found", Success = false };
        }

        public async Task<ApiPostResponse<List<LeaveTypeModel>>> GetLeaveTypeList()
        {
            var model = await _context.LeaveTypes.Where(_ => _.IsActive == true && _.IsDeleted == false).ToListAsync();
            if (model != null)
            {
                var leaveTypes = _mapper.Map<List<LeaveTypeModel>>(model);
                return new ApiPostResponse<List<LeaveTypeModel>> { Data = leaveTypes, Message = "Leave Type List found", Success = true };
            }
            return new ApiPostResponse<List<LeaveTypeModel>> { Message = "Leave Type List not found", Success = false };
        }

        public async Task<BaseApiResponse> ToggleStatusLeaveType(long id)
        {
            var model = await _context.LeaveTypes.FirstOrDefaultAsync(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false);
            if (model != null)
            {
                model.IsDeleted = !model.IsActive;
                model.DeletedBy = _userSession.Current.UserId;
                _context.LeaveTypes.Update(model);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return new BaseApiResponse { Message = "Leave Type status changed", Success = true };
                else
                    return new BaseApiResponse { Message = "Leave Type not status changed", Success = false };
            }
            return new BaseApiResponse { Message = "Leave Type not found", Success = false };
        }
    }
}
