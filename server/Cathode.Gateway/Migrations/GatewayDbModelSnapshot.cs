// <auto-generated />
using System;
using Cathode.Gateway;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Cathode.Gateway.Migrations
{
    [DbContext(typeof(GatewayDb))]
    partial class GatewayDbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Cathode.Common.Settings.SettingsEntry<Cathode.Gateway.GatewaySetting>", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<GatewaySetting>("Value")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pk_settings");

                    b.ToTable("settings");
                });

            modelBuilder.Entity("Cathode.Gateway.Index.Node", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("AccountId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("account_id");

                    b.Property<string>("AcmeChallenge")
                        .HasColumnType("text")
                        .HasColumnName("acme_challenge");

                    b.Property<string>("AuthenticationToken")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("authentication_token");

                    b.Property<string>("ControlToken")
                        .HasColumnType("text")
                        .HasColumnName("control_token");

                    b.Property<Guid>("ControlTokenChallenge")
                        .HasColumnType("uuid")
                        .HasColumnName("control_token_challenge");

                    b.Property<Guid>("DeviceId")
                        .HasColumnType("uuid")
                        .HasColumnName("device_id");

                    b.Property<DateTime>("FirstSeen")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("first_seen");

                    b.Property<DateTime>("LastSeen")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("last_seen");

                    b.Property<string>("LookupToken")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("lookup_token");

                    b.HasKey("Id")
                        .HasName("pk_nodes");

                    b.HasIndex("AccountId")
                        .HasDatabaseName("ix_nodes_account_id");

                    b.HasIndex("AccountId", "DeviceId")
                        .IsUnique()
                        .HasDatabaseName("ix_nodes_account_id_device_id");

                    b.ToTable("nodes");
                });

            modelBuilder.Entity("Cathode.Gateway.Index.NodeConnectionInformation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("address");

                    b.Property<Guid>("NodeId")
                        .HasColumnType("uuid")
                        .HasColumnName("node_id");

                    b.Property<int>("Priority")
                        .HasColumnType("integer")
                        .HasColumnName("priority");

                    b.HasKey("Id")
                        .HasName("pk_node_connection_information");

                    b.HasIndex("NodeId", "Address")
                        .IsUnique()
                        .HasDatabaseName("ix_node_connection_information_node_id_address");

                    b.ToTable("node_connection_information");
                });

            modelBuilder.Entity("Cathode.Gateway.Index.NodeConnectionInformation", b =>
                {
                    b.HasOne("Cathode.Gateway.Index.Node", "Node")
                        .WithMany("ConnectionInfo")
                        .HasForeignKey("NodeId")
                        .HasConstraintName("fk_node_connection_information_nodes_node_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Node");
                });

            modelBuilder.Entity("Cathode.Gateway.Index.Node", b =>
                {
                    b.Navigation("ConnectionInfo");
                });
#pragma warning restore 612, 618
        }
    }
}
