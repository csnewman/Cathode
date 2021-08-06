using Cathode.Common.Database;
using Cathode.Common.Settings;
using Cathode.Gateway.Index;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cathode.Gateway
{
    public class GatewayDb : DbContext, ISettingsDbProvider<GatewaySetting>
    {
        public DbSet<Node> Nodes { get; set; }

        public DbSet<SettingsEntry<GatewaySetting>> Settings { get; set; }

        static GatewayDb()
        {
            DatabaseUtils.ConfigureJson();
        }

        public GatewayDb([NotNull] DbContextOptions options) : base(options)
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
            mb.Entity<Node>(b =>
            {
                b.HasIndex(x => new { x.AccountId, x.DeviceId })
                    .IsUnique();

                b.HasIndex(x => new { x.AccountId });

                b.HasMany(x => x.ConnectionInfo)
                    .WithOne(c => c.Node)
                    .HasForeignKey(c => c.NodeId);
            });

            mb.Entity<NodeConnectionInformation>(b =>
            {
                b.HasIndex(x => new { x.NodeId, x.Address })
                    .IsUnique();
            });

            SettingsEntry<GatewaySetting>.OnModelCreating(mb);
        }
    }
}