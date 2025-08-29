using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class TeamMember
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long TeamId { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Employee Employee { get; set; }

    public virtual Team Team { get; set; }
}
