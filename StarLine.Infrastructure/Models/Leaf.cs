using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class Leaf
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long LeaveTypeId { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public int TotalDays { get; set; }

    public int Status { get; set; }

    public string RejectReason { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public long IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Employee Employee { get; set; }
}
