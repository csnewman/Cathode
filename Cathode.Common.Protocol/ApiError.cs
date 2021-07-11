using System.Collections.Generic;

namespace Cathode.Common.Protocol
{
    public class ApiError
    {
        public ApiErrorCode Code { get; set; }

        public string Message { get; set; } = null!;

        public IDictionary<string, string[]>? ValidationErrors { get; set; }

        public ApiError()
        {
        }

        public ApiError(ApiErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}