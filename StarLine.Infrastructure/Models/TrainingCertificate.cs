using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class TrainingCertificate
{
    public long Id { get; set; }

    public string CertificateNo { get; set; }

    public long EmployeeId { get; set; }

    public long TrainingAssignedId { get; set; }

    public DateTime? IssuedDate { get; set; }

    public DateTime? ValidUntil { get; set; }

    public string CertificatePath { get; set; }

    public long IssuedBy { get; set; }

    public string Remarks { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Employee Employee { get; set; }

    public virtual TrainingAssignment TrainingAssigned { get; set; }
}
