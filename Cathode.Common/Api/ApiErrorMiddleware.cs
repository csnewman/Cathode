using System;
using System.Threading.Tasks;
using Cathode.Common.Protocol;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Cathode.Common.Api
{
    [UsedImplicitly]
    public class ApiErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ApiErrorMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ApiErrorMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                // Check if we are missing a status code body
                if (context.Response.HasStarted
                    || context.Response.StatusCode < 400
                    || context.Response.StatusCode >= 600
                    || context.Response.ContentLength.HasValue
                    || !string.IsNullOrEmpty(context.Response.ContentType))
                {
                    return;
                }

                await SendResponseAsync(
                    context,
                    new ApiResult<object>(
                        context.Response.StatusCode,
                        context.Response.StatusCode switch
                        {
                            StatusCodes.Status400BadRequest => new ApiError(ApiErrorCode.BadRequest),
                            StatusCodes.Status401Unauthorized => new ApiError(ApiErrorCode.Unauthorised),
                            StatusCodes.Status403Forbidden => new ApiError(ApiErrorCode.Forbidden),
                            StatusCodes.Status404NotFound => new ApiError(ApiErrorCode.NotFound),
                            StatusCodes.Status415UnsupportedMediaType => new ApiError(ApiErrorCode.UnsupportedMediaType),
                            StatusCodes.Status500InternalServerError => new ApiError(ApiErrorCode.InternalError),
                            _ => new ApiError(
                                ApiErrorCode.Unknown,
                                $"Unknown response {context.Response.StatusCode}"
                            )
                        }
                    )
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred while executing the request");
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("Response has already started, unable to update error response");
                    throw;
                }

                try
                {
                    // Prevent data from leaking, such as set headers
                    context.Response.Clear();

                    var environment = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
                    await SendResponseAsync(context, new ApiResult<object>(
                            StatusCodes.Status404NotFound,
                            new ApiError
                            {
                                Code = ApiErrorCode.InternalError,
                                Message = environment.IsDevelopment()
                                    ? ex.Message
                                    : "An internal server error has occured"
                            }
                        )
                    );
                }
                catch (Exception ex2)
                {
                    // Suppress secondary exceptions, re-throw the original
                    _logger.LogError(ex2, "An occured while attempting to handle an API error");
                    throw;
                }
            }
        }

        private static Task SendResponseAsync(HttpContext context, JsonResult result)
        {
            context.Response.OnStarting(ClearCacheHeaders, context.Response);

            var executor = context.RequestServices.GetRequiredService<IActionResultExecutor<JsonResult>>();
            return executor.ExecuteAsync(new ActionContext(context, new RouteData(), new ActionDescriptor()), result);
        }

        private static Task ClearCacheHeaders(object state)
        {
            var headers = ((HttpResponse)state).Headers;
            headers[HeaderNames.CacheControl] = "no-cache";
            headers[HeaderNames.Pragma] = "no-cache";
            headers[HeaderNames.Expires] = "-1";
            headers.Remove(HeaderNames.ETag);
            return Task.CompletedTask;
        }
    }
}