using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class ShiftGroupMapping
{
    public long Id { get; set; }

    public long ShiftGroupId { get; set; }

    public long ShiftId { get; set; }

    public int SequenceNo { get; set; }

    public int? RotationDays { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Shift Shift { get; set; }

    public virtual ShiftGroup ShiftGroup { get; set; }
}
