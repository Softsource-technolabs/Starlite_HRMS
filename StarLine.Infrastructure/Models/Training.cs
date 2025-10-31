using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class Training
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Category { get; set; }

    public string Description { get; set; }

    public int Duration { get; set; }

    public int ValidityMonths { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();
}
