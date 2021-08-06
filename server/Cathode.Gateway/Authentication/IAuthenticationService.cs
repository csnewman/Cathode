using System;
using System.Threading.Tasks;

namespace Cathode.Gateway.Authentication
{
    public interface IAuthenticationService
    {
        Task<bool> AuthenticateNodeAsync(Guid registrationId, string authenticationToken);
    }
}