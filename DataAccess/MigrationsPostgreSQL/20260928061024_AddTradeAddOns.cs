using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.MigrationsPostgreSQL
{
    /// <inheritdoc />
    public partial class AddTradeAddOns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TradeAddOns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BaseTradeId = table.Column<int>(type: "integer", nullable: false),
                    EntryPrice = table.Column<double>(type: "double precision", nullable: true),
                    ExitPrice = table.Column<double>(type: "double precision", nullable: true),
                    Volume = table.Column<double>(type: "double precision", nullable: true),
                    PnL = table.Column<double>(type: "double precision", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeAddOns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeAddOns_BaseTrades_BaseTradeId",
                        column: x => x.BaseTradeId,
                        principalTable: "BaseTrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TradeAddOns_BaseTradeId",
                table: "TradeAddOns",
                column: "BaseTradeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeAddOns");
        }
    }
}
