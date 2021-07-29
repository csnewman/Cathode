using System;
using Cathode.Common;
using Microsoft.Extensions.Configuration;

namespace Cathode.Gateway
{
    public class GatewayOptions
    {
        public string DatabaseConnectionString { get; }

        public bool AcmeEnabled { get; }

        public string? AcmeServer { get; }

        public string? AcmeEmail { get; }

        public string? AcmeDomain { get; }

        public GatewayOptions(IConfiguration configuration)
        {
            DatabaseConnectionString = configuration.ParseString(null, "Database", "ConnectionString") ??
                                       throw new Exception("Database connection string missing");

            AcmeEnabled = configuration.ParseBool(false, "Acme", "Enabled");
            if (AcmeEnabled)
            {
                AcmeServer = configuration.ParseString(null, "Acme", "Server") ??
                             throw new Exception("Acme server address missing");
                AcmeEmail = configuration.ParseString(null, "Acme", "Email") ??
                            throw new Exception("Acme email address missing");
                AcmeDomain = configuration.ParseString(null, "Acme", "Domain") ??
                             throw new Exception("Acme domain missing");
            }
        }
    }
}