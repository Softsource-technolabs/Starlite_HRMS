using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class Team
{
    public long Id { get; set; }

    public string Name { get; set; }

    public long DepartmentId { get; set; }

    public int TeamType { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Department Department { get; set; }

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
