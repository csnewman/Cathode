using System.Security.Claims;

namespace Cathode.Gateway.Authentication
{
    public class NodeAuthEntity : IAuthEntity
    {
        public string RegistrationId { get; }
        public string AccountId { get; }
        public string DeviceId { get; }

        public NodeAuthEntity(ClaimsPrincipal claims)
        {
            RegistrationId = claims.FindFirstValue(GatewayClaims.RegistrationId);
            AccountId = claims.FindFirstValue(GatewayClaims.RegistrationId);
            DeviceId = claims.FindFirstValue(GatewayClaims.RegistrationId);
        }
    }
}