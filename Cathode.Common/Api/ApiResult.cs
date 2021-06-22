using Cathode.Common.Protocol;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cathode.Common.Api
{
    public class ApiResult<T> : JsonResult where T : ApiResponse, new()
    {
        public ApiResult(int statusCode, T value) : base(value)
        {
            StatusCode = statusCode;
        }
    }

    public static class ApiResultHelper
    {
        public static ApiResult<T> Success<T>(T obj) where T : ApiResponse, new()
        {
            obj.Success = true;
            return new ApiResult<T>(StatusCodes.Status200OK, obj);
        }

        public static ApiResult<T> BadRequest<T>(T obj) where T : ApiResponse, new()
        {
            obj.Success = false;
            return new ApiResult<T>(StatusCodes.Status400BadRequest, obj);
        }

        public static ApiResult<T> Forbidden<T>(T obj) where T : ApiResponse, new()
        {
            obj.Success = false;
            return new ApiResult<T>(StatusCodes.Status403Forbidden, obj);
        }

        public static ApiResult<T> NotFound<T>() where T : ApiResponse, new()
        {
            return new ApiResult<T>(StatusCodes.Status404NotFound, new T
            {
                Success = false,
                ErrorMessage = "Content not found"
            });
        }
    }
}