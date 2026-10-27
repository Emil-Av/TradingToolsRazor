using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.MigrationsPostgreSQL
{
    /// <inheritdoc />
    public partial class AddTomSessionTrader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Session",
                table: "TomStreams",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Trader",
                table: "TomStreams",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Session",
                table: "TomStreams");

            migrationBuilder.DropColumn(
                name: "Trader",
                table: "TomStreams");
        }
    }
}
