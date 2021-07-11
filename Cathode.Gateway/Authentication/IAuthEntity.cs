using System.Security.Claims;

namespace Cathode.Gateway.Authentication
{
    public interface IAuthEntity
    {
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