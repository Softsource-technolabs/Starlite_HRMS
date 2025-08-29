namespace StarLine.Core.Common
{
    public class PagedResponse<T>
    {
        public T Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int FilteredRecord { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        public PagedResponse(T data, int pageNumber, int pageSize, int totalRecords,int filteredRecord)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            FilteredRecord = filteredRecord;
        }
    }
}
