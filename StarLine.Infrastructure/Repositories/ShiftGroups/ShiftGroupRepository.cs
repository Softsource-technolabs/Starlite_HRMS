using AutoMapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;

namespace StarLine.Infrastructure.Repositories.Shifts
{
    public class ShiftGroupRepository : IShiftGroupRepository
    {
        private readonly StarLiteContext _context;
        private readonly IMapper _mapper;
        private readonly IUserSession _userSession;
        public ShiftGroupRepository(StarLiteContext context, IUserSession userSession, IMapper mapper)
        {
            _context = context;
            _userSession = userSession;
            _mapper = mapper;
        }
        public async Task<ApiPostResponse<long>> AddShiftGroup(ShiftGroupModel model)
        {
            var shiftgroup = _mapper.Map<ShiftGroup>(model);
            await _context.ShiftGroups.AddAsync(shiftgroup);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new ApiPostResponse<long> { Data = shiftgroup.Id, Message = "Shift Group Created", Success = true };
            else
                return new ApiPostResponse<long> { Message = "Error while creating shift group", Success = false };
        }
        public async Task<BaseApiResponse> DeleteShiftGroup(long id)
        {
            var model = await _context.ShiftGroups.FirstOrDefaultAsync(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false);
            if (model != null)
            {
                model.IsDeleted = false;
                model.DeletedBy = _userSession.Current.UserId;
                model.DeletedDate = DateTime.UtcNow;
                _context.ShiftGroups.Update(model);

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return new BaseApiResponse { Success = true, Message = "Shift group deleted" };
                else
                    return new BaseApiResponse { Success = false, Message = "Shift group not deleted" };
            }
            return new BaseApiResponse { Success = false, Message = "Shift group not found" };
        }
        public async Task<ApiPostResponse<List<ShiftGroupModel>>> GetAllShiftGroups()
        {
            var shiftgroups = await _context.ShiftGroups.Where(_ => _.IsActive == true && _.IsDeleted == false).ToListAsync();
            if (shiftgroups != null)
            {
                var model = _mapper.Map<List<ShiftGroupModel>>(shiftgroups);
                return new ApiPostResponse<List<ShiftGroupModel>> { Data = model, Success = true, Message = "Shift group list found" };
            }
            return new ApiPostResponse<List<ShiftGroupModel>> { Success = false, Message = "Shift group list not found" };
        }
        public async Task<ApiPostResponse<ShiftGroupModel>> GetShiftGroup(long id)
        {
            var shiftGroup = await _context.ShiftGroups.FirstOrDefaultAsync(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false);
            if (shiftGroup != null)
            {
                var model = _mapper.Map<ShiftGroupModel>(shiftGroup);
                return new ApiPostResponse<ShiftGroupModel> { Data = model, Success = true, Message = "Shift group found" };
            }
            return new ApiPostResponse<ShiftGroupModel> { Success = false, Message = "Shift group not found" };
        }

        public async Task<List<ShiftGroupModel>> GetShiftGroupList()
        {
            var result = await _context.ShiftGroups.Where(_ => _.IsActive == true && _.IsDeleted == false).ToListAsync();
            return _mapper.Map<List<ShiftGroupModel>>(result);
        }

        public async Task<BaseApiResponse> ToggleStatus(long id)
        {
            var model = await _context.ShiftGroups.FirstOrDefaultAsync(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false);
            if (model != null)
            {
                model.IsActive = !model.IsActive;
                model.DeletedBy = _userSession.Current.UserId;
                model.DeletedDate = DateTime.UtcNow;
                _context.ShiftGroups.Update(model);

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return new BaseApiResponse { Success = true, Message = "Shift group status changed" };
                else
                    return new BaseApiResponse { Success = false, Message = "Shift group status not changed" };
            }
            return new BaseApiResponse { Success = false, Message = "Shift group not found" };
        }
        public async Task<ApiPostResponse<long>> UpdateShiftGroup(long id, ShiftGroupModel shiftGroup)
        {
            var existShiftGroup = await _context.ShiftGroups.FirstOrDefaultAsync(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false);
            if (existShiftGroup != null)
            {
                var model = _mapper.Map(shiftGroup, existShiftGroup);
                model.UpdatedBy = _userSession.Current.UserId;
                _context.ShiftGroups.Update(model);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return new ApiPostResponse<long> { Data = model.Id, Success = true, Message = "Shift group Updated" };
                else
                    return new ApiPostResponse<long> { Success = false, Message = "Shift group not Updated" };
            }
            return new ApiPostResponse<long> { Success = false, Message = "Shift group not found" };
        }
    }
}

