using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccess.MigrationsPostgreSQL
{
    /// <inheritdoc />
    public partial class AddScreenshotTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScreenshotImports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Imported = table.Column<int>(type: "integer", nullable: false),
                    Missing = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenshotImports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Screenshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BaseTradeId = table.Column<int>(type: "integer", nullable: true),
                    TomTradeId = table.Column<int>(type: "integer", nullable: true),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LegacyPath = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Screenshots", x => x.Id);
                    table.CheckConstraint("CK_Screenshots_OneOwner", "(\"BaseTradeId\" IS NULL) <> (\"TomTradeId\" IS NULL)");
                    table.ForeignKey(
                        name: "FK_Screenshots_BaseTrades_BaseTradeId",
                        column: x => x.BaseTradeId,
                        principalTable: "BaseTrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Screenshots_TomTrades_TomTradeId",
                        column: x => x.TomTradeId,
                        principalTable: "TomTrades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScreenshotContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenshotContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScreenshotContents_Screenshots_Id",
                        column: x => x.Id,
                        principalTable: "Screenshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Screenshots_BaseTradeId_Position",
                table: "Screenshots",
                columns: new[] { "BaseTradeId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Screenshots_TomTradeId_Position",
                table: "Screenshots",
                columns: new[] { "TomTradeId", "Position" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScreenshotContents");

            migrationBuilder.DropTable(
                name: "ScreenshotImports");

            migrationBuilder.DropTable(
                name: "Screenshots");
        }
    }
}
