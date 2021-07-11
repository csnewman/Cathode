using System.Collections.Generic;
using System.Linq;
using Cathode.Common.Protocol;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cathode.Common.Api
{
    public class ApiModelErrorFactory
    {
        public static IActionResult ConvertError(ActionContext ctx)
        {
            var validationErrors = new Dictionary<string, string[]>();

            foreach (var (key, value) in ctx.ModelState)
            {
                var errors = value.Errors;
                if (errors == null || errors.Count <= 0) continue;

                var errorMessages = errors
                    .Select(x => string.IsNullOrEmpty(x.ErrorMessage) ? "Invalid input" : x.ErrorMessage).ToArray();

                validationErrors.Add(key, errorMessages);
            }

            return new ApiResult<object>(
                StatusCodes.Status400BadRequest,
                new ApiError
                {
                    Code = ApiErrorCode.ValidationFailed,
                    Message = "Request validation failed",
                    ValidationErrors = validationErrors
                }
            );
        }
    }
}