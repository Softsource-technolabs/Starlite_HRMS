using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class Shift
{
    public long Id { get; set; }

    public long ShiftGroupId { get; set; }

    public string ShiftCode { get; set; }

    public string ShiftName { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public decimal WorkingHours { get; set; }

    public bool IsNightShift { get; set; }

    public int? GracePeriodMins { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<ShiftGroupMapping> ShiftGroupMappings { get; set; } = new List<ShiftGroupMapping>();
}
