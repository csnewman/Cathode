namespace Cathode.Common.Protocol
{
    public class ApiResponse<T> where T : new()
    {
        public bool Success { get; set; }

        public ApiError? Error { get; set; }

        public T? Data { get; set; }
    }
}