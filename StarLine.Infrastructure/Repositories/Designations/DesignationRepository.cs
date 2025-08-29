using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;
using System.Reflection;

namespace StarLine.Infrastructure.Repositories.Designations
{
    public class DesignationRepository(StarLiteContext context, IMapper mapper, IUserSession userSession) : IDesignationRepository
    {
        private readonly StarLiteContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserSession _userSession = userSession;

        public async Task<BaseApiResponse> AddDesignation(DesignationModel designation)
        {
            var model = _mapper.Map<Designation>(designation);
            model.CreatedBy = _userSession.Current.UserId;
            await _context.Designations.AddAsync(model);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new BaseApiResponse { Message = "Designation added", Success = true };
            else
                return new BaseApiResponse { Message = "Designation not added", Success = false };
        }

        public async Task<BaseApiResponse> DeleteDesignation(long id)
        {
            var model = await _context.Designations.Where(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                model.DeletedBy = _userSession.Current.UserId;
                model.IsDeleted = true;
                _context.Designations.Update(model);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Message = "Designation deleted", Success = true };
                }
            }
            return new BaseApiResponse { Message = "Designation not deleted", Success = false };
        }

        public async Task<PagedResponse<List<DesignationModel>>> GetAllDesignations(PaginationModel model)
        {
            var query = _context.Designations.Include(_ => _.Department).Where(_ => _.IsDeleted == false).AsQueryable();
            var totalRecord = query.Count();
            if (!string.IsNullOrEmpty(model.StrSearch))
            {
                query = query.Where(_ => _.DesignationName.Contains(model.StrSearch) || _.Description.Contains(model.StrSearch) || _.Department.DepartmentName.Contains(model.StrSearch));
            }

            if (!string.IsNullOrWhiteSpace(model.SortOrder))
            {
                var property = typeof(Designation).GetProperty(model.SortColumn ?? "Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                string columnName = property != null ? property.Name : "Id";
                bool isDescending = model.SortOrder.ToLower() == "desc";
                query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, columnName))
                    : query.OrderBy(e => EF.Property<object>(e, columnName));
            }
            var count = await query.CountAsync();
            var data = await query.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToListAsync();
            var modelData = _mapper.Map<List<DesignationModel>>(data);
            return new PagedResponse<List<DesignationModel>>(modelData, model.PageNumber, model.PageSize, totalRecord, count);
        }

        public async Task<List<DesignationModel>> GetDesignationByDepartment(long departmentId)
        {
            var model = await _context.Designations.Where(_ => _.DepartmentId == departmentId && _.IsActive == true && _.IsDeleted == false).ToListAsync();
            if(model != null)
            {
                return _mapper.Map<List<DesignationModel>>(model);
            }
            return null;
        }

        public async Task<ApiPostResponse<DesignationModel>> GetDesignationById(long id)
        {
            var model = await _context.Designations.Where(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                var Designation = _mapper.Map<DesignationModel>(model);
                return new ApiPostResponse<DesignationModel> { Data = Designation, Success = true, Message = "Designation Found" };
            }
            return new ApiPostResponse<DesignationModel> { Success = false, Message = "Designation not Found" };
        }

        public async Task<ApiPostResponse<List<DesignationModel>>> GetDesignationList()
        {
            var model = await _context.Designations.Where(_ => _.IsActive == true && _.IsDeleted == false).ToListAsync();
            if (model != null)
            {
                var Designations = _mapper.Map<List<DesignationModel>>(model);
                return new ApiPostResponse<List<DesignationModel>> { Data = Designations, Success = true, Message = "Designations Found" };
            }
            return new ApiPostResponse<List<DesignationModel>> { Success = true, Message = "Designations not Found" };
        }

        public async Task<BaseApiResponse> ToggleStatusDesignation(long id)
        {
            var model = await _context.Designations.Where(_ => _.Id == id && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                model.UpdatedBy = _userSession.Current.UserId;
                model.IsActive = !model.IsActive;
                _context.Designations.Update(model);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Message = "Designation Activation status Changed", Success = true };
                }
            }
            return new BaseApiResponse { Message = "Designation not deleted", Success = false };
        }

        public async Task<BaseApiResponse> UpdateDesignation(DesignationModel designation)
        {
            var model = await _context.Designations.Where(_ => _.Id == designation.Id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                _context.Entry(model).CurrentValues.SetValues(designation);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Success = true, Message = "Designation updated" };
                }
                return new BaseApiResponse { Success = false, Message = "Designation not updated" };
            }
            return new BaseApiResponse { Success = false, Message = "Designation not found" };
        }
    }
}
