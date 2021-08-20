using System;
using Cathode.Common;
using Microsoft.Extensions.Configuration;

namespace Cathode.MediaServer
{
    public class ServerOptions
    {
        public string DatabaseConnectionString { get; }

        public ServerOptions(IConfiguration configuration)
        {
            DatabaseConnectionString = configuration.ParseString(null, "Database", "ConnectionString") ??
                                       throw new Exception("Database connection string missing");
        }
    }
}