using System;
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

        public ApiError(ApiErrorCode code)
        {
            Code = code;
            Message = GetDefaultMessage(code);
        }

        public static string GetDefaultMessage(ApiErrorCode code)
        {
            return code switch
            {
                ApiErrorCode.Unknown => "Unknown error",
                ApiErrorCode.InternalError => "An internal server error has occured",
                ApiErrorCode.UnsupportedApiVersion => "The provided api version is unsupported",
                ApiErrorCode.InvalidApiVersion => "The provided api version is invalid",
                ApiErrorCode.ValidationFailed => "Request validation failed",
                ApiErrorCode.BadRequest => "Bad request",
                ApiErrorCode.Unauthorised => "Authentication failed",
                ApiErrorCode.Forbidden => "Authentication failed",
                ApiErrorCode.NotFound => "The requested content could not be found",
                ApiErrorCode.UnsupportedMediaType => "The supplied media type is unsupported",
                ApiErrorCode.AlreadyInUse => "The provided details are already in use",
                ApiErrorCode.AlreadyActivated => "Activation has already been completed",
                ApiErrorCode.NotConfigured => "Configuration has not been completed",
                _ => throw new ArgumentOutOfRangeException(nameof(code), code, null)
            };
        }
    }
}