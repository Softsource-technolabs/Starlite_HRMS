using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;
using System.Reflection;

namespace StarLine.Infrastructure.Repositories.Departments
{
    public class DepartmentRepository(StarLiteContext context, IMapper mapper, IUserSession userSession) : IDepartmentRepository
    {
        private readonly StarLiteContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserSession _userSession = userSession;

        public async Task<BaseApiResponse> AddDepartment(DepartmentModel department)
        {
            var model = _mapper.Map<Department>(department);
            model.CreatedBy = _userSession.Current.UserId;
            await _context.Departments.AddAsync(model);
            var result = await _context.SaveChangesAsync();
            if(result > 0)
                return new BaseApiResponse { Message = "Department added",Success = true};
            else
                return new BaseApiResponse { Message = "Department not added", Success = false };
        }

        public async Task<BaseApiResponse> DeleteDepartment(long id)
        {
            var model = await _context.Departments.Where(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                model.DeletedBy = _userSession.Current.UserId;
                model.IsDeleted = true;
                _context.Departments.Update(model);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Message = "Department deleted", Success = true };
                }
            }
            return new BaseApiResponse { Message = "Department not deleted", Success = false };
        }

        public async Task<PagedResponse<List<DepartmentModel>>> GetAllDepartments(PaginationModel model)
        {
            var query = _context.Departments.Where(_ =>  _.IsDeleted == false).AsQueryable();
            var totalRecord = query.Count();
            if (!string.IsNullOrEmpty(model.StrSearch))
            {
                query = query.Where(_ => _.DepartmentName.Contains(model.StrSearch) || _.Description.Contains(model.StrSearch));
            }

            if (!string.IsNullOrWhiteSpace(model.SortOrder))
            {
                var property = typeof(Department).GetProperty(model.SortColumn ?? "Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                string columnName = property != null ? property.Name : "Id";
                bool isDescending = model.SortOrder.ToLower() == "desc";
                query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, columnName))
                    : query.OrderBy(e => EF.Property<object>(e, columnName));
            }

            var count = await query.CountAsync();
            var data = await query.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToListAsync();
            var modelData = _mapper.Map<List<DepartmentModel>>(data);
            return new PagedResponse<List<DepartmentModel>>(modelData, model.PageNumber, model.PageSize, totalRecord, count);
        }

        public async Task<ApiPostResponse<DepartmentModel>> GetDepartmentById(long id)
        {
            var model = await _context.Departments.Where(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                var department = _mapper.Map<DepartmentModel>(model);
                return new ApiPostResponse<DepartmentModel> { Data = department,Success = true,Message = "Department Found"};
            }
            return new ApiPostResponse<DepartmentModel> { Success = false, Message = "Department not Found" };
        }

        public async Task<ApiPostResponse<List<DepartmentModel>>> GetDepartmentList()
        {
            var model = await _context.Departments.Where(_ => _.IsActive == true && _.IsDeleted == false).ToListAsync();
            if(model != null)
            {
                var departments = _mapper.Map<List<DepartmentModel>>(model);
                return new ApiPostResponse<List<DepartmentModel>> { Data = departments, Success = true, Message = "Departments Found" };
            }
            return new ApiPostResponse<List<DepartmentModel>> { Success = true, Message = "Departments not Found" };
        }

        public async Task<BaseApiResponse> ToggleStatusDepartment(long id)
        {
            var model = await _context.Departments.Where(_ => _.Id == id && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                model.UpdatedBy = _userSession.Current.UserId;
                model.IsActive = !model.IsActive;
                _context.Departments.Update(model);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Message = "Department Activation status Changed", Success = true };
                }
            }
            return new BaseApiResponse { Message = "Department not deleted", Success = false };
        }

        public async Task<BaseApiResponse> UpdateDepartment(DepartmentModel department)
        {
            var model = await _context.Departments.Where(_ => _.Id == department.Id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                var dept = _mapper.Map(department, model);
                dept.UpdatedBy = _userSession.Current.UserId;
                _context.Departments.Update(dept);
                var result = await _context.SaveChangesAsync();
                if(result > 0)
                {
                    return new BaseApiResponse { Success = true, Message = "Department updated" };
                }
                return new BaseApiResponse { Success = false, Message = "Department not updated" };
            }
            return new BaseApiResponse { Success = false, Message = "Department not found" };
        }
    }
}
