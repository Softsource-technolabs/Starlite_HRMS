using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class Holiday
{
    public long Id { get; set; }

    public string Name { get; set; }

    public DateOnly HolidayDate { get; set; }

    public int HolidayType { get; set; }

    public string HolidayTypename { get; set; }

    /// <summary>
    /// According To law if leave is mandatory
    /// </summary>
    public bool Ismandatory { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }
}
