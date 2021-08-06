using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cathode.Gateway.Authentication
{
    public class CathodeAuthenticationHandler : AuthenticationHandler<CathodeAuthenticationOptions>
    {
        private const string AuthorizationHeaderName = "Authorization";
        private const string BearerSchemeName = "Bearer";
        public const string AuthenticationScheme = "Cathode";
        private readonly IAuthenticationService _authenticationService;

        public CathodeAuthenticationHandler(
            IOptionsMonitor<CathodeAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder,
            ISystemClock clock, IAuthenticationService authenticationService) : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticationService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out var headerValue))
            {
                return AuthenticateResult.NoResult();
            }

            if (!BearerSchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrEmpty(headerValue.Parameter))
            {
                return AuthenticateResult.NoResult();
            }

            var parts = headerValue.Parameter.Split(":", 2);
            if (parts.Length != 2)
            {
                return AuthenticateResult.Fail("Invalid authentication header format");
            }

            return parts[0].ToLower() switch
            {
                "node" => await ValidateNodeToken(parts[1]),
                _ => AuthenticateResult.Fail("Invalid authentication target")
            };
        }

        private async Task<AuthenticateResult> ValidateNodeToken(string token)
        {
            var parts = token.Split(":", 2);
            if (parts.Length != 2)
            {
                return AuthenticateResult.Fail("Invalid node token format");
            }

            if (!Guid.TryParse(parts[0], out var registrationId) || string.IsNullOrEmpty(parts[1]))
            {
                return AuthenticateResult.Fail("Invalid node token format");
            }

            if (!await _authenticationService.AuthenticateNodeAsync(registrationId, parts[1]))
            {
                return AuthenticateResult.Fail("Invalid node token");
            }

            var principal = IAuthEntity.CreateNodePrincipal(Scheme.Name, registrationId);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}