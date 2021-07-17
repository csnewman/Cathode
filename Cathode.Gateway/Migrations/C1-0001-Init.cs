using System;
using Cathode.Gateway;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cathode.Gateway.Migrations
{
    public partial class _0001Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "nodes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<string>(type: "text", nullable: false),
                    device_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lookup_token = table.Column<string>(type: "text", nullable: false),
                    authentication_token = table.Column<string>(type: "text", nullable: false),
                    control_token_challenge = table.Column<Guid>(type: "uuid", nullable: false),
                    control_token = table.Column<string>(type: "text", nullable: true),
                    first_seen = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    last_seen = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_nodes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<GatewaySetting>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "node_connection_information",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    node_id = table.Column<Guid>(type: "uuid", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_node_connection_information", x => x.id);
                    table.ForeignKey(
                        name: "fk_node_connection_information_nodes_node_id",
                        column: x => x.node_id,
                        principalTable: "nodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_node_connection_information_node_id_address",
                table: "node_connection_information",
                columns: new[] { "node_id", "address" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_nodes_account_id",
                table: "nodes",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_nodes_account_id_device_id",
                table: "nodes",
                columns: new[] { "account_id", "device_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "node_connection_information");

            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropTable(
                name: "nodes");
        }
    }
}
