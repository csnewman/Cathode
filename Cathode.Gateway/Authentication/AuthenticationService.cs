using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Cathode.Gateway.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly GatewayDb _db;

        public AuthenticationService(GatewayDb db)
        {
            _db = db;
        }

        public Task<bool> AuthenticateNodeAsync(Guid registrationId, string authenticationToken)
        {
            return _db.Nodes.AnyAsync(x => x.Id == registrationId && x.AuthenticationToken == authenticationToken);
        }
    }
}