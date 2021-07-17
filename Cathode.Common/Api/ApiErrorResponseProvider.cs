using System;
using Cathode.Common.Protocol;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Cathode.Common.Api
{
    public class ApiErrorResponseProvider : IErrorResponseProvider
    {
        public IActionResult CreateResponse(ErrorResponseContext context)
        {
            return new ApiResult<object>(StatusCodes.Status400BadRequest, context.ErrorCode switch
            {
                ErrorCodes.UnsupportedApiVersion => new ApiError(ApiErrorCode.UnsupportedApiVersion),
                ErrorCodes.AmbiguousApiVersion or ErrorCodes.ApiVersionUnspecified or ErrorCodes.InvalidApiVersion =>
                    new ApiError(ApiErrorCode.InvalidApiVersion),
                _ => throw new ArgumentOutOfRangeException()
            });
        }
    }
}