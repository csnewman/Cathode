using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Cathode.Gateway.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticationSettings> GetSettingsAsync();

        Task<RSA> GetPrivateKeyAsync();

        Task<string> GenerateTokenAsync(IEnumerable<Claim> claims, TimeSpan expire);
    }
}