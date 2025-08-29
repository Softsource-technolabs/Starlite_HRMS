namespace StarLine.Core.Common
{
    public class PaginationModel
    {
        public int draw { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string StrSearch { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
    }
}
