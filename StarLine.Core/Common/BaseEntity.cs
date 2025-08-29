namespace StarLine.Core.Common
{
    public class BaseEntity
    {
        public long Id { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
