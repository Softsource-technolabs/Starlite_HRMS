using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;
using System.Reflection;

namespace StarLine.Infrastructure.Repositories.Holidays
{
    public class HolidayRepository : IHolidayRepository
    {
        private readonly StarLiteContext _context;
        private readonly IMapper _mapper;
        private readonly IUserSession _userSession;

        public HolidayRepository(StarLiteContext context, IMapper mapper, IUserSession userSession)
        {
            _context = context;
            _mapper = mapper;
            _userSession = userSession;
        }
        public async Task<BaseApiResponse> AddHoliday(HolidayModel holiday)
        {
            var model = _mapper.Map<Holiday>(holiday);
            model.CreatedBy = _userSession.Current.UserId;
            await _context.Holidays.AddAsync(model);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new BaseApiResponse { Message = "Holiday added", Success = true };
            else
                return new BaseApiResponse { Message = "Holiday not added", Success = false };
        }

        public async Task<BaseApiResponse> DeleteHoliday(long id)
        {
            var model = await _context.Holidays.Where(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                model.DeletedBy = _userSession.Current.UserId;
                model.IsDeleted = true;
                _context.Holidays.Update(model);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Message = "Holiday deleted", Success = true };
                }
            }
            return new BaseApiResponse { Message = "Holiday not deleted", Success = false };
        }

        public async Task<PagedResponse<List<HolidayModel>>> GetAllHolidays(PaginationModel model)
        {
            var query = _context.Holidays.Where(_ => _.IsDeleted == false).AsQueryable();
            var totalRecord = query.Count();
            if (!string.IsNullOrEmpty(model.StrSearch))
            {
                query = query.Where(_ => _.Name.Contains(model.StrSearch));
            }

            if (!string.IsNullOrWhiteSpace(model.SortOrder))
            {
                var property = typeof(Holiday).GetProperty(model.SortColumn ?? "Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                string columnName = property != null ? property.Name : "Id";
                bool isDescending = model.SortOrder.ToLower() == "desc";
                query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, columnName))
                    : query.OrderBy(e => EF.Property<object>(e, columnName));
            }
            var count = await query.CountAsync();
            var data = await query.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToListAsync();
            var modelData = _mapper.Map<List<HolidayModel>>(data);
            return new PagedResponse<List<HolidayModel>>(modelData, model.PageNumber, model.PageSize, totalRecord, count);
        }

        public async Task<ApiPostResponse<HolidayModel>> GetHolidayById(long id)
        {
            var model = await _context.Holidays.FirstOrDefaultAsync(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false);
            if (model != null)
            {
                var holiday = _mapper.Map<HolidayModel>(model);
                return new ApiPostResponse<HolidayModel> { Data = holiday, Message = "Holiday found", Success = true };
            }
            return new ApiPostResponse<HolidayModel> { Message = "Holiday not found", Success = true };
        }

        public async Task<BaseApiResponse> ToggleStatusHoliday(long id)
        {
            var model = await _context.Holidays.Where(_ => _.Id == id && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                model.UpdatedBy = _userSession.Current.UserId;
                model.IsActive = !model.IsActive;
                _context.Holidays.Update(model);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Message = "Holiday Activation status Changed", Success = true };
                }
            }
            return new BaseApiResponse { Message = "Holiday not deleted", Success = false };
        }

        public async Task<BaseApiResponse> UpdateHoliday(HolidayModel holiday)
        {
            var model = await _context.Holidays.Where(_ => _.Id == holiday.Id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                _context.Entry(model).CurrentValues.SetValues(holiday);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Success = true, Message = "Holiday updated" };
                }
                return new BaseApiResponse { Success = false, Message = "Holiday not updated" };
            }
            return new BaseApiResponse { Success = false, Message = "Holiday not found" };
        }
    }
}
