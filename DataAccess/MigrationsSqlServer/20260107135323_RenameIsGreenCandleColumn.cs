using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RenameIsGreenCandleColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGreenCandle",
                table: "SRS");

            migrationBuilder.AddColumn<int>(
                name: "CandleType",
                table: "SRS",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CandleType",
                table: "SRS");

            migrationBuilder.AddColumn<bool>(
                name: "IsGreenCandle",
                table: "SRS",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
