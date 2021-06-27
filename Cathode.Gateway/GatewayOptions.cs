using System;
using Cathode.Common;
using Microsoft.Extensions.Configuration;

namespace Cathode.Gateway
{
    public class GatewayOptions
    {
        public string DatabaseConnectionString { get; set; }

        public GatewayOptions(IConfiguration configuration)
        {
            DatabaseConnectionString = configuration.ParseString(null, "Database", "ConnectionString") ??
                                       throw new Exception("Database connection string missing");
        }
    }
}