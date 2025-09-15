using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class Branch
{
    public long Id { get; set; }

    public string BranchCode { get; set; }

    public string BranchName { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string Country { get; set; }

    public string ContactNumber { get; set; }

    public string EmailAddress { get; set; }

    public int? ParentBranchId { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }
}
