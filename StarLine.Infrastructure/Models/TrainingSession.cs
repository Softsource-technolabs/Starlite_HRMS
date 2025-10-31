using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class TrainingSession
{
    public long Id { get; set; }

    public long TrainingId { get; set; }

    public string TrainingCode { get; set; }

    public string Category { get; set; }

    public string Description { get; set; }

    public long TrainerId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? DurationHours { get; set; }

    public int? Mode { get; set; }

    public string Location { get; set; }

    public int? MaxParticipants { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Employee Trainer { get; set; }

    public virtual Training Training { get; set; }

    public virtual ICollection<TrainingAssignment> TrainingAssignments { get; set; } = new List<TrainingAssignment>();
}
