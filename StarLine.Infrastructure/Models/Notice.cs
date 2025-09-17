using System;
using System.Collections.Generic;

namespace StarLine.Infrastructure.Models;

public partial class Notice
{
    public long Id { get; set; }

    public string Title { get; set; }

    public string NoticeText { get; set; }

    public int NoticeType { get; set; }

    public int DeliveryMode { get; set; }

    public bool AcknoledgeRequired { get; set; }

    public bool HasAttachment { get; set; }

    public string FileName { get; set; }

    public string ActualFileName { get; set; }

    public int AudienceType { get; set; }

    public string AudienceTypeValue { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishDate { get; set; }

    public DateTime? ExpireDate { get; set; }

    public bool IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }
}
