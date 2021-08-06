using System;
using System.Security.Claims;

namespace Cathode.Gateway.Authentication
{
    public interface IAuthEntity
    {
        public static ClaimsPrincipal CreateNodePrincipal(string authenticationScheme, Guid registrationId)
        {
            var claims = new[]
            {
                new Claim(GatewayClaims.Type, GatewayClaims.NodeType),
                new Claim(GatewayClaims.RegistrationId, registrationId.ToString())
            };
            var identity = new ClaimsIdentity(claims, authenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        public static IAuthEntity? Parse(ClaimsPrincipal claims)
        {
            return claims.FindFirstValue(GatewayClaims.Type) switch
            {
                GatewayClaims.NodeType => new NodeAuthEntity(claims),
                _ => null
            };
        }
    }
}