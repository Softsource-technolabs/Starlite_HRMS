using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;
using System.Reflection;

namespace StarLine.Infrastructure.Repositories.Employees
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly StarLiteContext _context;
        private readonly IMapper _mapper;
        private readonly IUserSession _userSession;
        public EmployeeRepository(StarLiteContext context, IMapper mapper, IUserSession userSession)
        {
            _context = context;
            _mapper = mapper;
            _userSession = userSession;
        }

        public async Task<BaseApiResponse> AddEmployee(EmployeeModel employee)
        {
            var model = _mapper.Map<Employee>(employee);
            model.CreatedBy = _userSession.Current.UserId;
            model.LoginFirst = true;
            await _context.Employees.AddAsync(model);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
                return new BaseApiResponse { Message = "Employee added", Success = true };
            else
                return new BaseApiResponse { Message = "Employee not added", Success = false };
        }

        public async Task<BaseApiResponse> DeleteEmployee(long id)
        {
            var model = await _context.Employees.Where(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                model.DeletedBy = _userSession.Current.UserId;
                model.IsDeleted = true;
                _context.Employees.Update(model);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Message = "Employee deleted", Success = true };
                }
            }
            return new BaseApiResponse { Message = "Employee not deleted", Success = false };
        }

        public async Task<PagedResponse<List<EmployeeModel>>> GetAllEmployees(PaginationModel model)
        {
            var query = _context.Employees.Include(_ => _.AspNetUser).ThenInclude(_ => _.Roles).Where(_ => _.IsDeleted == false).AsQueryable();
            var totalRecord = query.Count();
            if (!string.IsNullOrEmpty(model.StrSearch))
            {
                query = query.Where(_ => _.EmployeeCode.Contains(model.StrSearch) || _.FirstName.Contains(model.StrSearch) || _.LastName.Contains(model.StrSearch)
                || _.Email.Contains(model.StrSearch) || _.PhoneNumber.Contains(model.StrSearch));
            }

            if (!string.IsNullOrWhiteSpace(model.SortOrder))
            {
                var property = typeof(Employee).GetProperty(model.SortColumn ?? "Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                string columnName = property != null ? property.Name : "Id";
                bool isDescending = model.SortOrder.ToLower() == "desc";
                query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, columnName))
                    : query.OrderBy(e => EF.Property<object>(e, columnName));
            }

            var count = await query.CountAsync();
            var data = await query.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToListAsync();
            var modelData = _mapper.Map<List<EmployeeModel>>(data);
            return new PagedResponse<List<EmployeeModel>>(modelData, model.PageNumber, model.PageSize, totalRecord, count);
        }

        public async Task<ApiPostResponse<EmployeeModel>> GetEmployeeByEmail(string emailAddress)
        {
            var model = await _context.Employees.Include(_ => _.AspNetUser).ThenInclude(_ => _.Roles).Where(_ => _.Email == emailAddress && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                var employee = _mapper.Map<EmployeeModel>(model);
                employee.RoleId = model.AspNetUser.Roles.FirstOrDefault().Id;
                return new ApiPostResponse<EmployeeModel> { Data = employee, Success = true, Message = "Employee Found" };
            }
            return new ApiPostResponse<EmployeeModel> { Success = false, Message = "Employee not Found" };
        }

        public async Task<ApiPostResponse<EmployeeModel>> GetEmployeeById(long id)
        {
            var model = await _context.Employees.Include(_ => _.AspNetUser).ThenInclude(_ => _.Roles).Where(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                var employee = _mapper.Map<EmployeeModel>(model);
                employee.RoleId = model.AspNetUser.Roles.FirstOrDefault().Id;
                return new ApiPostResponse<EmployeeModel> { Data = employee, Success = true, Message = "Employee Found" };
            }
            return new ApiPostResponse<EmployeeModel> { Success = false, Message = "Employee not Found" };
        }

        public async Task<ApiPostResponse<List<EmployeeModel>>> GetEmployeeList()
        {
            var model = await _context.Employees.Include(_ => _.AspNetUser).ThenInclude(_ => _.Roles).Where(_ => _.IsActive == true && _.IsDeleted == false).ToListAsync();
            if (model != null)
            {
                var Designations = _mapper.Map<List<EmployeeModel>>(model);
                return new ApiPostResponse<List<EmployeeModel>> { Data = Designations, Success = true, Message = "Employee Found" };
            }
            return new ApiPostResponse<List<EmployeeModel>> { Success = true, Message = "Employee not Found" };
        }

        public async Task<ApiPostResponse<List<EmployeeModel>>> GetManagersList()
        {
            string[] excludedRoles = { "Employee", "Super-Admin" };

            var model = await _context.Employees.Include(e => e.AspNetUser).ThenInclude(u => u.Roles)
                .Where(e => e.IsActive && !e.IsDeleted && e.AspNetUser.Roles.All(r => !excludedRoles.Contains(r.Name))).ToListAsync();
            if (model != null)
            {
                var users = _mapper.Map<List<EmployeeModel>>(model);
                return new ApiPostResponse<List<EmployeeModel>> { Data = users, Success = true, Message = "Managers Found" };
            }
            return new ApiPostResponse<List<EmployeeModel>> { Success = true, Message = "Managers not Found" };
        }

        public async Task<BaseApiResponse> ToggleStatusEmployee(long id)
        {
            var model = await _context.Employees.Where(_ => _.Id == id && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                model.UpdatedBy = _userSession.Current.UserId;
                model.IsActive = !model.IsActive;
                _context.Employees.Update(model);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Message = "Employee Activation status Changed", Success = true };
                }
            }
            return new BaseApiResponse { Message = "Employee not deleted", Success = false };
        }

        public async Task<BaseApiResponse> UpdateEmployee(EmployeeModel employee)
        {
            var model = await _context.Employees.Where(_ => _.Id == employee.Id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                var emp = _mapper.Map(employee, model);
                emp.UpdatedBy = _userSession.Current.UserId;
                emp.UpdatedDate = DateTime.UtcNow;
                _context.Employees.Update(emp);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new BaseApiResponse { Success = true, Message = "Employee updated" };
                }
                return new BaseApiResponse { Success = false, Message = "Employee not updated" };
            }
            return new BaseApiResponse { Success = false, Message = "Employee not found" };
        }
    }
}
