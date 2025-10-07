namespace StarLine.Infrastructure.Models;

public partial class Attendance
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public DateOnly Attendacedate { get; set; }

    public TimeOnly InTime { get; set; }

    public TimeOnly? OutTime { get; set; }

    public int Status { get; set; }

    public bool? LateComing { get; set; }

    public string Remarks { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Employee Employee { get; set; }
}
