using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Cathode.Common.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Cathode.Gateway.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly GatewayDb _db;
        private readonly ISettingsProvider<GatewayDb, GatewaySetting> _settingsProvider;
        private AuthenticationSettings? _settings;
        private RSA? _rsa;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public AuthenticationService(GatewayDb db, ISettingsProvider<GatewayDb, GatewaySetting> settingsProvider)
        {
            _db = db;
            _settingsProvider = settingsProvider;
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public async Task<AuthenticationSettings> GetSettingsAsync()
        {
            return _settings ??=
                await _settingsProvider.GetOrAddAsync(AuthenticationSettings.SettingId, InitializeSettings);
        }

        private static AuthenticationSettings InitializeSettings()
        {
            var key = RSA.Create(2048);
            var privateKey = key.ExportRSAPrivateKey();

            return new AuthenticationSettings
            {
                JwtIssuer = "cathode:gateway:" + Guid.NewGuid(),
                JwtPrivateKey = privateKey
            };
        }

        public async Task<RSA> GetPrivateKeyAsync()
        {
            if (_rsa != null)
            {
                return _rsa;
            }

            var settings = await GetSettingsAsync();
            _rsa = RSA.Create();
            _rsa.ImportRSAPrivateKey(settings.JwtPrivateKey, out _);
            return _rsa;
        }

        public async Task<string> GenerateTokenAsync(IEnumerable<Claim> claims, TimeSpan expire)
        {
            var settings = await GetSettingsAsync();
            var key = await GetPrivateKeyAsync();
            var now = DateTime.UtcNow;

            return _tokenHandler.WriteToken(new JwtSecurityToken(
                signingCredentials: new SigningCredentials(new RsaSecurityKey(key), SecurityAlgorithms.RsaSha512),
                claims: claims,
                notBefore: now,
                expires: now.Add(expire),
                issuer: settings.JwtIssuer
            ));
        }
    }
}