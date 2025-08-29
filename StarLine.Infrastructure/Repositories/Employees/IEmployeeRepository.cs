using StarLine.Core.Common;
using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Employees
{
    public interface IEmployeeRepository
    {
        Task<BaseApiResponse> AddEmployee(EmployeeModel employee);
        Task<BaseApiResponse> UpdateEmployee(EmployeeModel employee);
        Task<BaseApiResponse> ToggleStatusEmployee(long id);
        Task<BaseApiResponse> DeleteEmployee(long id);
        Task<ApiPostResponse<EmployeeModel>> GetEmployeeById(long id);
        Task<ApiPostResponse<EmployeeModel>> GetEmployeeByEmail(string emailAddress);
        Task<PagedResponse<List<EmployeeModel>>> GetAllEmployees(PaginationModel model);
        Task<ApiPostResponse<List<EmployeeModel>>> GetEmployeeList();
        Task<ApiPostResponse<List<EmployeeModel>>> GetManagersList();
    }
}
