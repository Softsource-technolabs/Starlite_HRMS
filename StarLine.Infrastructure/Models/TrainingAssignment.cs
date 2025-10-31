using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class TrainingAssignment
{
    public long Id { get; set; }

    public long TrainingSessionId { get; set; }

    public long EmployeeId { get; set; }

    public DateTime? AssignedDate { get; set; }

    public int Status { get; set; }

    public DateTime? CompletionDate { get; set; }

    public string Remarks { get; set; }

    public bool? IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeleetedDate { get; set; }

    public virtual Employee Employee { get; set; }

    public virtual ICollection<TrainingCertificate> TrainingCertificates { get; set; } = new List<TrainingCertificate>();

    public virtual TrainingSession TrainingSession { get; set; }
}
