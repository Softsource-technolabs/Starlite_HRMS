using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class LeaveType
{
    public long Id { get; set; }

    public string TypeName { get; set; }

    public string Description { get; set; }

    public int MaxDaysPerMonth { get; set; }

    public bool IsCarryForward { get; set; }

    public int GenderContraint { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }
}
