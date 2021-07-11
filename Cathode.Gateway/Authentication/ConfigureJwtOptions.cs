using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Cathode.Gateway.Authentication
{
    public class ConfigureJwtOptions : IConfigureOptions<JwtBearerOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ConfigureJwtOptions(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Configure(JwtBearerOptions options)
        {
            ConfigureAsync(options).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private async Task ConfigureAsync(JwtBearerOptions options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;
            var service = provider.GetRequiredService<IAuthenticationService>();
            var settings = await service.GetSettingsAsync();
            var privateKey = await service.GetPrivateKeyAsync();

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(privateKey),
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidIssuer = settings.JwtIssuer,
                RequireAudience = false,
                ValidateAudience = false
            };
        }
    }
}