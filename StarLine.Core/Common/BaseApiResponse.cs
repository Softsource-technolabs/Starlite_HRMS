namespace StarLine.Core.Common
{
    public class BaseApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public BaseApiResponse()
        {

        }
    }
    public class ApiPostResponse<T> : BaseApiResponse
    {
        public T Data { get; set; } = default!;
    }
}
