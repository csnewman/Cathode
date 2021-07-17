using System.Security.Claims;

namespace Cathode.Gateway.Authentication
{
    public class NodeAuthEntity : IAuthEntity
    {
        public string RegistrationId { get; }

        public NodeAuthEntity(ClaimsPrincipal claims)
        {
            RegistrationId = claims.FindFirstValue(GatewayClaims.RegistrationId);
        }
    }
}