namespace StarLine.Core.Common
{
    public class PagedResponse<T>
    {
        public T Data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }

        public PagedResponse(T data, int totalRecords, int filteredRecord)
        {
            Data = data;
            recordsFiltered = filteredRecord;
            recordsTotal = totalRecords;
        }
    }
}
