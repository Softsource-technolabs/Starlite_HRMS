using StarLine.Core.Common;

namespace StarLine.Core.Models
{
    public class TrainingAssignmentModel : BaseEntity
    {
        public long TrainingSessionId { get; set; }
        public string TrainingSessionName { get; set; }
        public string trainingCode { get; set; }
        public string trainerName { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? AssignedDate { get; set; }
        public int? Status { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Remarks { get; set; }
    }

    public class TrainingAssignmentListModel
    {
        public long Id { get; set; }
        public string TrainingName { get; set; }
        public string SessionCode { get; set; }
        public string TrainerName { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public DateTime? AssignedDate { get; set; }
        public TrainingAssignStatus CompletionStatus { get; set; }
    }
}
