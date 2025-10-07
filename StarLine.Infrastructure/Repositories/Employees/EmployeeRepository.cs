using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Core.Session;
using StarLine.Infrastructure.Models;
using StarLine.Infrastructure.Repositories.Shifts;
using System.Net.Mail;
using System.Reflection;

namespace StarLine.Infrastructure.Repositories.Employees
{
    public class EmployeeRepository(StarLiteContext context, IMapper mapper, IUserSession userSession, IShiftRepository shiftRepository) : IEmployeeRepository
    {
        private readonly StarLiteContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly IUserSession _userSession = userSession;
        private readonly IShiftRepository _shiftRepository = shiftRepository;

        public async Task<long> AddUpdateEmployee(EmployeeModel employee)
        {
            if (employee.Id > 0)
                return await AddEmployee(employee);
            else
                return await UpdateEmployee(employee);
        }
        private async Task<long> AddEmployee(EmployeeModel employee)
        {
            var shift = await _shiftRepository.GetShiftGeneralShift();
            if (shift == null)
                return -1;

            var model = _mapper.Map<Employee>(employee);
            model.ShiftId = shift.Id;
            model.CreatedBy = _userSession.Current.UserId;
            model.LoginFirst = true;
            await _context.Employees.AddAsync(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task<bool> checkforDuplicateBycode(string employeeCode)
        {
            return await _context.Employees.AnyAsync(_ => _.EmployeeCode == employeeCode);
        }

        public async Task<bool> checkforDuplicateByEmail(string emailAddress)
        {
            return await _context.Employees.AnyAsync(_ => _.Email == emailAddress);
        }

        public async Task<bool> checkforDuplicateByLicense(string licenseNo)
        {
            return await _context.Employees.AnyAsync(_ => _.LicenseNumber == licenseNo);
        }

        public async Task<bool> checkforDuplicateByMobile(string mobileNo)
        {
            return await _context.Employees.AnyAsync(_ => _.PhoneNumber == mobileNo);
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
            return new PagedResponse<List<EmployeeModel>>(modelData, totalRecord, count);
        }

        public async Task<ApiPostResponse<EmployeeModel>> GetEmployeeByEmail(string emailAddress)
        {
            var model = await _context.Employees.Include(_ => _.AspNetUser).ThenInclude(_ => _.Roles)
                .Include(_ => _.Shift).Where(_ => _.Email == emailAddress && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                var employee = _mapper.Map<EmployeeModel>(model);
                employee.RoleId = model.AspNetUser.Roles.FirstOrDefault().Id;
                employee.ShiftNameandTiming = model.Shift != null ? $"{model.Shift.ShiftName} - {model.Shift.StartTime} to {model.Shift.EndTime}" : "";
                return new ApiPostResponse<EmployeeModel> { Data = employee, Success = true, Message = "Employee Found" };
            }
            return new ApiPostResponse<EmployeeModel> { Success = false, Message = "Employee not Found" };
        }

        public async Task<ApiPostResponse<EmployeeModel>> GetEmployeeById(long id)
        {
            var model = await _context.Employees.Include(_ => _.AspNetUser).ThenInclude(_ => _.Roles).Include(_ => _.Shift).Where(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                var employee = _mapper.Map<EmployeeModel>(model);
                employee.RoleId = model.AspNetUser.Roles.FirstOrDefault().Id;
                employee.ShiftNameandTiming = model.Shift != null ? $"{model.Shift.ShiftName} - {model.Shift.StartTime} to {model.Shift.EndTime}" : "";
                return new ApiPostResponse<EmployeeModel> { Data = employee, Success = true, Message = "Employee Found" };
            }
            return new ApiPostResponse<EmployeeModel> { Success = false, Message = "Employee not Found" };
        }

        public async Task<EmployeeDetailsModel> GetEmployeeDetailsById(long id)
        {
            var employeeDetails = await _context.Employees.Include(_ => _.AspNetUser).ThenInclude(_ => _.Roles)
                .Include(_ => _.Department).ThenInclude(_ => _.Designations).FirstOrDefaultAsync(_ => _.Id == id);
            return new EmployeeDetailsModel
            {
                BloodGroup = Convert.ToInt32(employeeDetails.BloodGroup),
                CurrentAddress = employeeDetails.CurrentAddress,
                DateOfBirth = employeeDetails.DateOfBirth.ToString("dd MMM yyyy"),
                Department = employeeDetails.Department.DepartmentName,
                Designation = employeeDetails.Department.Designations.FirstOrDefault().DesignationName,
                Email = employeeDetails.Email,
                EmergencyContactName = employeeDetails.EmergencyContactName,
                EmergencyContactNumber = employeeDetails.EmergencyContactNumber,
                EmployeeCode = employeeDetails.EmployeeCode,
                EmploymentType = employeeDetails.EmploymentType,
                ExperienceInYears = (decimal)employeeDetails.ExperienceInYears,
                FirstName = employeeDetails.FirstName,
                Gender = employeeDetails.Gender,
                Id = employeeDetails.Id,
                IsActive = employeeDetails.IsActive,
                JoiningDate = employeeDetails.JoiningDate.ToString("dd MMM yyyy"),
                LastName = employeeDetails.LastName,
                LicenseNumber = employeeDetails.LicenseNumber,
                PermanentAddress = employeeDetails.PermanentAddress,
                PhoneNumber = employeeDetails.PhoneNumber,
                Qualification = employeeDetails.Qualification,
                ReportingManagerId = employeeDetails.ReportingManagerId,
                UserImages = employeeDetails.UserImages,
                RoleName = employeeDetails.AspNetUser.Roles.FirstOrDefault().Name,
            };
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
            var managerList = new List<EmployeeModel>();
            var model = await _context.Employees.Include(e => e.AspNetUser).ThenInclude(u => u.Roles).Include(_ => _.Designation)
                .Where(e => e.IsActive && !e.IsDeleted && e.AspNetUser.Roles.All(r => !excludedRoles.Contains(r.Name))
                && e.Designation.DesignationName.ToLower().Contains("head")).ToListAsync();
            if (model != null)
            {
                managerList = _mapper.Map<List<EmployeeModel>>(model);
                return new ApiPostResponse<List<EmployeeModel>> { Data = managerList, Success = true, Message = "Managers Found" };
            }
            else
            {
                var HrModel = await _context.Employees.Include(e => e.AspNetUser).ThenInclude(u => u.Roles).Include(_ => _.Designation)
                .Where(e => e.IsActive && !e.IsDeleted && e.AspNetUser.Roles.All(r => !excludedRoles.Contains(r.Name))
                && e.Designation.DesignationName == "HR Head / HR Manager").ToListAsync();
                managerList = _mapper.Map<List<EmployeeModel>>(HrModel);
                return new ApiPostResponse<List<EmployeeModel>> { Data = managerList, Success = true, Message = "Managers Found" };
            }
        }

        public async Task<List<EmployeeDetailsModel>> GetReportingManagers(long designationId)
        {
            var designation = await _context.Designations.FirstOrDefaultAsync(_ => _.Id == designationId);

            if (designation == null) return null;

            int empLevel = (int)designation.HierarchyLevel;

            var employeeDetails = await _context.Employees.Include(_ => _.AspNetUser).ThenInclude(_ => _.Roles)
                .Include(_ => _.Department).ThenInclude(_ => _.Designations)
                .Where(_ => _.DepartmentId == designation.DepartmentId && _.Designation.HierarchyLevel < empLevel).ToListAsync();

            if (employeeDetails.Count > 0)
            {
                return employeeDetails.Select(_ => new EmployeeDetailsModel
                {
                    BloodGroup = Convert.ToInt32(_.BloodGroup),
                    CurrentAddress = _.CurrentAddress,
                    DateOfBirth = _.DateOfBirth.ToString("MM-dd-yyyy"),
                    Department = _.Department.DepartmentName,
                    Designation = _.Designation.DesignationName,
                    Email = _.Email,
                    EmergencyContactName = _.EmergencyContactName,
                    EmergencyContactNumber = _.EmergencyContactNumber,
                    EmployeeCode = _.EmployeeCode,
                    EmploymentType = _.EmploymentType,
                    ExperienceInYears = _.ExperienceInYears != null ? Convert.ToInt64(_.ExperienceInYears) : 0,
                    FirstName = _.FirstName,
                    Gender = _.Gender,
                    Id = _.Id,
                    JoiningDate = _.JoiningDate.ToString("MM-dd-yyyy"),
                    LastName = _.LastName,
                    LicenseNumber = _.LicenseNumber,
                    PermanentAddress = _.PermanentAddress,
                    PhoneNumber = _.PhoneNumber,
                    Qualification = _.Qualification,
                    ReportingManagerId = _.ReportingManagerId,
                    RoleName = _.AspNetUser.Roles.FirstOrDefault()?.Name,
                    UserImages = _.UserImages,
                    ShiftNameandTiming = _.Shift != null ? $"{_.Shift.ShiftName} - {_.Shift.StartTime} to {_.Shift.EndTime}" : ""
                }).ToList();
            }
            else
            {
                var hrManager = await _context.Employees.Include(_ => _.AspNetUser).ThenInclude(_ => _.Roles)
                .Include(_ => _.Department).ThenInclude(_ => _.Designations)
                .Where(_ => _.Designation.DesignationName == "HR Head / HR Manager" || _.Department.DepartmentName == "Top Management").ToListAsync();

                if (hrManager != null)
                {
                    return hrManager.Select(_ => new EmployeeDetailsModel
                    {
                        BloodGroup = Convert.ToInt32(_.BloodGroup),
                        CurrentAddress = _.CurrentAddress,
                        DateOfBirth = _.DateOfBirth.ToString("MM-dd-yyyy"),
                        Department = _.Department.DepartmentName,
                        Designation = _.Designation.DesignationName,
                        Email = _.Email,
                        EmergencyContactName = _.EmergencyContactName,
                        EmergencyContactNumber = _.EmergencyContactNumber,
                        EmployeeCode = _.EmployeeCode,
                        EmploymentType = _.EmploymentType,
                        ExperienceInYears = _.ExperienceInYears != null ? Convert.ToInt64(_.ExperienceInYears) : 0,
                        FirstName = _.FirstName,
                        Gender = _.Gender,
                        Id = _.Id,
                        JoiningDate = _.JoiningDate.ToString("MM-dd-yyyy"),
                        LastName = _.LastName,
                        LicenseNumber = _.LicenseNumber,
                        PermanentAddress = _.PermanentAddress,
                        PhoneNumber = _.PhoneNumber,
                        Qualification = _.Qualification,
                        ReportingManagerId = _.ReportingManagerId,
                        RoleName = _.AspNetUser.Roles.FirstOrDefault()?.Name,
                        UserImages = _.UserImages,
                    }).ToList();
                }
            }
            return null;
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

        private async Task<long> UpdateEmployee(EmployeeModel employee)
        {
            var model = await _context.Employees.Where(_ => _.Id == employee.Id && _.IsActive == true && _.IsDeleted == false).FirstOrDefaultAsync();
            if (model != null)
            {
                var emp = _mapper.Map(employee, model);
                emp.UpdatedBy = _userSession.Current.UserId;
                emp.UpdatedDate = DateTime.UtcNow;
                _context.Employees.Update(emp);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<List<EmployeeModel>> GetEmployeeListForAssignTeam()
        {
            var currentUser = _userSession.Current.UserId;

            var employee = await _context.Employees.Include(d => d.Department).Where(_ => _.ReportingManagerId == currentUser).ToListAsync();
            if (employee != null)
            {
                return _mapper.Map<List<EmployeeModel>>(employee);
            }
            return null;
        }
    }
}
