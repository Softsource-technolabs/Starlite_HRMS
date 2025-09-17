using StarLine.Infrastructure.Repositories.Branches;
using StarLine.Infrastructure.Repositories.Departments;
using StarLine.Infrastructure.Repositories.Designations;
using StarLine.Infrastructure.Repositories.Employees;
using StarLine.Infrastructure.Repositories.Holidays;
using StarLine.Infrastructure.Repositories.LeaveTypes;
using StarLine.Infrastructure.Repositories.Lists;
using StarLine.Infrastructure.Repositories.Notices;
using StarLine.Infrastructure.Repositories.Shifts;
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
                { typeof(IShiftGroupRepository), typeof(ShiftGroupRepository) },
                { typeof(IShiftRepository), typeof(ShiftRepository) },
                { typeof(IBranchRepository), typeof(BranchRepository) },
                { typeof(INoticeRepository), typeof(NoticeRepository) },
                { typeof(ILookUpRepository), typeof(LookUpRepository) },
            };
            return repositoryDictionary;
        }
    }
}
