using System;
using Cathode.Common.Protocol;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cathode.Common.Api
{
    public class ApiErrorResponseProvider : IErrorResponseProvider
    {
        public IActionResult CreateResponse(ErrorResponseContext context)
        {
            return new ApiResult<GenericApiErrorResponse>(context.StatusCode, new GenericApiErrorResponse
            {
                Success = false,
                ErrorCode = string.IsNullOrEmpty(context.ErrorCode)
                    ? "unknown"
                    : char.ToLowerInvariant(context.ErrorCode[0]) + context.ErrorCode.Substring(1),
                ErrorMessage = NullIfEmpty(context.Message),
                ErrorDetails = NewInnerError(context, c => c.MessageDetail),
            });
        }

        static string? NullIfEmpty(string value) => string.IsNullOrEmpty(value) ? null : value;

        static TError NewInnerError<TError>(ErrorResponseContext context, Func<ErrorResponseContext, TError> create)
        {
            if (string.IsNullOrEmpty(context.MessageDetail))
            {
                return default!;
            }

            var environment = context.Request.HttpContext.RequestServices.GetService<IWebHostEnvironment>();
            return environment?.IsDevelopment() == true ? create(context) : default;
        }
    }
}