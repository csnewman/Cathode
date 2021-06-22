using System.Collections.Generic;
using Cathode.Common.Protocol;

namespace Cathode.Common.Api
{
    public class ApiValidationErrorResponse : ApiResponse
    {
        public string ErrorCode { get; }

        public IDictionary<string, string[]> ValidationErrors { get; }

        public ApiValidationErrorResponse()
        {
        }

        public ApiValidationErrorResponse(string errorCode, string errorMessage,
            IDictionary<string, string[]> validationErrors)
        {
            Success = false;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            ValidationErrors = validationErrors;
        }
    }
}