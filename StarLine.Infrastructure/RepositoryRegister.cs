using StarLine.Infrastructure.Repositories.Departments;
using StarLine.Infrastructure.Repositories.Designations;
using StarLine.Infrastructure.Repositories.Employees;
using StarLine.Infrastructure.Repositories.Holidays;
using StarLine.Infrastructure.Repositories.LeaveTypes;
using StarLine.Infrastructure.Repositories.Teams;

namespace StarLine.Infrastructure
{
    public static class RepositoryRegister
    {
        public static Dictionary<Type, Type> GetTypes()
        {
            var repositoryDictionary = new Dictionary<Type, Type>
            {
                { typeof(IEmployeeRepository), typeof(EmployeeRepository) },
                { typeof(IDepartmentRepository), typeof(DepartmentRepository) },
                { typeof(IDesignationRepository), typeof(DesignationRepository) },
                { typeof(ITeamRepository), typeof(TeamRepository) },
                { typeof(IHolidayRepository), typeof(HolidayRepository) },
                { typeof(ILeaveTypeRepository), typeof(LeaveTypeRepository) },
            };
            return repositoryDictionary;
        }
    }
}
