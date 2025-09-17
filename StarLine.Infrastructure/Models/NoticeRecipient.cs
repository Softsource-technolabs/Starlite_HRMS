using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class NoticeRecipient
{
    public long Id { get; set; }

    public long NoticeId { get; set; }

    public long UserId { get; set; }

    public bool IsAcknoledged { get; set; }

    public DateTime ReadAt { get; set; }

    public DateTime AcknowledgeDate { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }
}
