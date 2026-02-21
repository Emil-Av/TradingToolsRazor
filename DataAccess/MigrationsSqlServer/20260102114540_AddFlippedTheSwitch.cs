using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddFlippedTheSwitch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderType",
                table: "BaseTrades");

            migrationBuilder.AddColumn<bool>(
                name: "IsFlippedTheSwitch",
                table: "SRS",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFlippedTheSwitch",
                table: "SRS");

            migrationBuilder.AddColumn<int>(
                name: "OrderType",
                table: "BaseTrades",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
