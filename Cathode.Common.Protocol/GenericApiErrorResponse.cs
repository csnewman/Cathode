namespace Cathode.Common.Protocol
{
    public class GenericApiErrorResponse : ApiResponse
    {
        public string ErrorCode { get; set; }

        public string ErrorDetails { get; set; }

        public GenericApiErrorResponse()
        {
        }

        public GenericApiErrorResponse(string errorCode, string errorMessage, string errorDetails = null)
        {
            Success = false;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            ErrorDetails = errorDetails;
        }
    }
}