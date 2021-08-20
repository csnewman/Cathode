using Cathode.Common.Database;
using Cathode.Common.Settings;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cathode.MediaServer
{
    public class ServerDb : DbContext, ISettingsDbProvider<ServerSetting>
    {

        public DbSet<SettingsEntry<ServerSetting>> Settings { get; set; }

        static ServerDb()
        {
            DatabaseUtils.ConfigureJson();
        }

        public ServerDb([NotNull] DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder
                .UseSnakeCaseNamingConvention()
                .ReplaceService<IMigrationsIdGenerator, CathodeMigrationsIdGenerator>();
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            SettingsEntry<ServerSetting>.OnModelCreating(mb);
        }
    }
}