using StarLine.Core.Common;

namespace StarLine.Core.Models
{
    public class AttendanceModel : BaseEntity
    {
        public long EmployeeId { get; set; }
        public DateOnly Attendacedate { get; set; }
        public TimeOnly InTime { get; set; }
        public TimeOnly? OutTime { get; set; }
        public AttendaceStatus Status { get; set; }
        public bool LateComing { get; set; }
        public string Remarks { get; set; }
        public string StatusName => CommonFunctions.GetDisplayName<AttendaceStatus>((int)Status);
        public string EmployeeName { get; set; }
    }
}
