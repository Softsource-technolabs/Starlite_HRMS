using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Employees
{
    public interface IEmployeeRepository
    {
        Task<long> AddUpdateEmployee(EmployeeModel employee);
        Task<BaseApiResponse> ToggleStatusEmployee(long id);
        Task<BaseApiResponse> DeleteEmployee(long id);
        Task<ApiPostResponse<EmployeeModel>> GetEmployeeById(long id);
        Task<ApiPostResponse<EmployeeModel>> GetEmployeeByEmail(string emailAddress);
        Task<PagedResponse<List<EmployeeModel>>> GetAllEmployees(PaginationModel model);
        Task<ApiPostResponse<List<EmployeeModel>>> GetEmployeeList();
        Task<ApiPostResponse<List<EmployeeModel>>> GetManagersList();
        Task<EmployeeDetailsModel> GetEmployeeDetailsById(long id);
        Task<List<EmployeeDetailsModel>> GetReportingManagers(long designationId);
        Task<bool> checkforDuplicateBycode(string employeeCode);
        Task<bool> checkforDuplicateByEmail(string emailAddress);
        Task<bool> checkforDuplicateByMobile(string mobileNo);
        Task<bool> checkforDuplicateByLicense(string licenseNo);
        Task<List<EmployeeModel>> GetTeamEmployee();
        Task<bool> UpdateEmployeeTeamAssign(EmployeeTeamAssignModel model);
    }
}
