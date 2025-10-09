namespace StarLine.Core.Models
{
    public class EmployeeTeamAssignModel
    {
        public long EmployeeId { get; set; }
        public long DepartmentId { get; set; }
        public long DesignationId { get; set; }
        public long ReportingManagerId { get; set; }
        public long TeamId { get; set; }
        public long ShiftGroupId { get; set; }
        public long ShiftId { get; set; }

    }
}
