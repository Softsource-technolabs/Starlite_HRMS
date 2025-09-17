using StarLine.Core.Common;

namespace StarLine.Core.Models
{
    public class NoticeRecipientModel : BaseEntity
    {
        public long NoticeId { get; set; }
        public long UserId { get; set; }
        public bool IsAcknoledged { get; set; }
        public DateTime ReadAt { get; set; }
        public DateTime AcknowledgeDate { get; set; }
    }
}
