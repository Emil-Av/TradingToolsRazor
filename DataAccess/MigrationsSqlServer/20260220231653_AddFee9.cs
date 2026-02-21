using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.MigrationsSqlServer
{
    /// <inheritdoc />
    public partial class AddFee9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Fee",
                table: "BaseTrades",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fee",
                table: "BaseTrades");
        }
    }
}
