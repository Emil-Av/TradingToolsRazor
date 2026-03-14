using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.MigrationsSqlServer
{
    /// <inheritdoc />
    public partial class RemoveTriggerPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TriggerPrice",
                table: "BaseTrades");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TriggerPrice",
                table: "BaseTrades",
                type: "float",
                nullable: true);
        }
    }
}
