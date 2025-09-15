using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class ShiftGroup
{
    public long Id { get; set; }

    public string GroupCode { get; set; }

    public string GroupName { get; set; }

    public int RotationType { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<ShiftGroupMapping> ShiftGroupMappings { get; set; } = new List<ShiftGroupMapping>();
}
