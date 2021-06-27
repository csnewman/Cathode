using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_id = table.Column<string>(type: "text", nullable: false),
                    device_id = table.Column<string>(type: "text", nullable: false),
                    first_seen = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    last_seen = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_nodes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "node_connection_information",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    node_id = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    last_seen = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
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
                name: "nodes");
        }
    }
}
