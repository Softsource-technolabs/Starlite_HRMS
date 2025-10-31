using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class TransferRequest
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long FromDepartmentId { get; set; }

    public long ToDepartmentId { get; set; }

    public int CurrentManagerApproval { get; set; }

    public int ReceivingManagerApproval { get; set; }

    public int Hrapproval { get; set; }

    public string Reason { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Employee Employee { get; set; }

    public virtual Department FromDepartment { get; set; }

    public virtual Department ToDepartment { get; set; }
}
