using StarLine.Core.Models;

namespace StarLine.Infrastructure.Repositories.Attendace
{
    public interface IAttendanceRepository
    {
        Task<long> AddUpdateAttendace(AttendanceModel model);
        Task<AttendanceModel> GetAttendanceById(long id);
        Task<AttendanceModel> GetAttendanceByEmpId(long empId);
        Task<List<AttendanceModel>> GetAttendanceList(long empId);
    }
}
