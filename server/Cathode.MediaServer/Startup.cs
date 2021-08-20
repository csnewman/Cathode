using Cathode.Common.Api;
using Cathode.Common.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Cathode.MediaServer
{
    public class Startup
    {
        private readonly ServerOptions _options;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _options = new ServerOptions(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_options);

            services
                .AddDbContext<ServerDb>(options => { options.UseNpgsql(_options.DatabaseConnectionString); })
                .AddScoped<ISettingsProvider<ServerDb, ServerSetting>, SettingsProvider<ServerDb, ServerSetting>>()
                .AddScoped<IAuthenticationService, AuthenticationService>();

            // services
            //     .AddAuthorization()
            //     .AddAuthentication(x =>
            //     {
            //         x.DefaultAuthenticateScheme = CathodeAuthenticationHandler.AuthenticationScheme;
            //         x.DefaultChallengeScheme = CathodeAuthenticationHandler.AuthenticationScheme;
            //     })
            //     .AddScheme<CathodeAuthenticationOptions, CathodeAuthenticationHandler>(
            //         CathodeAuthenticationHandler.AuthenticationScheme,
            //         _ => { }
            //     );

            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                    options.SerializerSettings.ContractResolver = ApiContractResolver.Instance;
                })
                .ConfigureApiBehaviorOptions(x =>
                {
                    x.InvalidModelStateResponseFactory = ApiModelErrorFactory.ConvertError;
                    x.SuppressMapClientErrors = true;
                });

            services.AddApiVersioning(options =>
            {
                options.UseApiBehavior = true;
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = false;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.ErrorResponses = new ApiErrorResponseProvider();
            });

            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ApiErrorMiddleware>();

            if (!env.IsDevelopment()) app.UseHsts();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}