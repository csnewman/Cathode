using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cathode.Gateway.Certificates
{
    public class AcmeMiddleware : IMiddleware
    {
        public const string BasePath = "/.well-known/acme-challenge";
        private readonly IAcmeManager _acmeManager;
        private readonly ILogger<AcmeMiddleware> _logger;

        public AcmeMiddleware(
            IAcmeManager acmeManager,
            ILogger<AcmeMiddleware> logger)
        {
            _acmeManager = acmeManager;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = context.Request.Path.ToString();
            if (token.StartsWith("/"))
            {
                token = token[1..];
            }

            if (!_acmeManager.TryGetChallenge(token, out var response))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = "Invalid token";
                _logger.LogInformation("Invalid ACME challenge request for {}", token);
            }
            else
            {
                _logger.LogInformation("Completed ACME challenge request for {}", token);
            }

            context.Response.ContentLength = response!.Length;
            context.Response.ContentType = "application/octet-stream";
            await context.Response.WriteAsync(response, context.RequestAborted);
        }
    }
}