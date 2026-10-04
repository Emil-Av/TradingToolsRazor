using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.MigrationsPostgreSQL
{
    /// <inheritdoc />
    public partial class AddSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountType = table.Column<int>(type: "integer", nullable: false),
                    AccountAmount = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquityResets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountType = table.Column<int>(type: "integer", nullable: false),
                    ResetAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartingBalance = table.Column<double>(type: "double precision", nullable: false),
                    PreviousStartingBalance = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquityResets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InstrumentSpreads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Symbol = table.Column<string>(type: "text", nullable: false),
                    Spread = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstrumentSpreads", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountSettings_AccountType",
                table: "AccountSettings",
                column: "AccountType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquityResets_AccountType_ResetAt",
                table: "EquityResets",
                columns: new[] { "AccountType", "ResetAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentSpreads_Symbol",
                table: "InstrumentSpreads",
                column: "Symbol",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountSettings");

            migrationBuilder.DropTable(
                name: "EquityResets");

            migrationBuilder.DropTable(
                name: "InstrumentSpreads");
        }
    }
}
