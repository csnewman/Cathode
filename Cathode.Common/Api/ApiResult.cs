using Cathode.Common.Protocol;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cathode.Common.Api
{
    public class ApiResult<T> : JsonResult where T : new()
    {
        public ApiResult(int statusCode, T value) : base(new ApiResponse<T>
        {
            Success = true,
            Data = value
        })
        {
            StatusCode = statusCode;
        }

        public ApiResult(int statusCode, ApiError error) : base(new ApiResponse<T>
        {
            Success = false,
            Error = error
        })
        {
            StatusCode = statusCode;
        }
    }

    public static class ApiResultHelper
    {
        public static ApiResult<T> Success<T>(T obj) where T : new()
        {
            return new ApiResult<T>(StatusCodes.Status200OK, obj);
        }

        public static ApiResult<T> BadRequest<T>(ApiError? error = null) where T : new()
        {
            return new ApiResult<T>(
                StatusCodes.Status400BadRequest,
                error ?? new ApiError(ApiErrorCode.BadRequest, "Bad request")
            );
        }

        public static ApiResult<T> Forbidden<T>(ApiError? error = null) where T : new()
        {
            return new ApiResult<T>(
                StatusCodes.Status403Forbidden,
                error ?? new ApiError(ApiErrorCode.Forbidden, "Authentication failed")
            );
        }
    }
}