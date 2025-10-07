using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarLine.Core.Common;
using StarLine.Core.Models;
using StarLine.Infrastructure.Models;

namespace StarLine.Infrastructure.Repositories.Attendace
{
    public class AttendanceRepository(StarLiteContext context, IMapper mapper) : IAttendanceRepository
    {
        private readonly StarLiteContext _context = context;
        private readonly IMapper _mapper = mapper;
        public async Task<long> AddUpdateAttendace(AttendanceModel model)
        {
            if (model.Id > 0)
                return await UpdateAttendance(model);
            else
                return await AddAttendance(model);
        }

        public async Task<AttendanceModel> GetAttendanceByEmpId(long empId)
        {
            var attendance = await _context.Attendances.Where(_ => _.EmployeeId == empId && _.IsActive == true 
            && _.IsDeleted == false && _.Attendacedate == DateOnly.FromDateTime(DateTime.Now)).ToListAsync();
            if (attendance != null)
            {
                var lastCheckIn = attendance.FirstOrDefault(_ => _.OutTime == null);
                if (lastCheckIn != null)
                {
                    return _mapper.Map<AttendanceModel>(lastCheckIn);
                }
                else
                {
                    var lastattendance = attendance.LastOrDefault(_ => _.OutTime != null);
                    return _mapper.Map<AttendanceModel>(lastattendance);
                }
            }
            return null;
        }

        public async Task<AttendanceModel> GetAttendanceById(long id)
        {
            var attendance = await _context.Attendances.FirstOrDefaultAsync(_ => _.Id == id && _.IsActive == true && _.IsDeleted == false);
            if (attendance != null)
                return _mapper.Map<AttendanceModel>(attendance);
            return null;
        }

        public async Task<List<AttendanceModel>> GetAttendanceList(long empId)
        {
            return await _context.Attendances.Include(_ => _.Employee).Where(_ => _.EmployeeId == empId && _.IsActive == true 
            && _.IsDeleted == false && _.Attendacedate == DateOnly.FromDateTime(DateTime.Now))
                .Select(_ => new AttendanceModel
                {
                    Attendacedate = _.Attendacedate,
                    EmployeeId = _.EmployeeId,
                    EmployeeName = _.Employee.FirstName + " " + _.Employee.LastName,
                    InTime = _.InTime,
                    Id = _.Id,
                    OutTime = _.OutTime,
                    Status = (AttendaceStatus)_.Status,
                }).ToListAsync();
        }

        private async Task<long> AddAttendance(AttendanceModel model)
        {
            var attendance = _mapper.Map<Attendance>(model);
            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
            return attendance.Id;
        }

        private async Task<long> UpdateAttendance(AttendanceModel model)
        {
            var existingAttendance = await _context.Attendances.FirstOrDefaultAsync(_ => _.Id == model.Id && _.IsActive == true && _.IsDeleted == false);
            if (existingAttendance != null)
            {
                var newAttendance = _mapper.Map(model, existingAttendance);
                _context.Attendances.Update(newAttendance);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }
    }
}
