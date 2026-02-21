using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBaseTrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Targets",
                table: "BaseTrades");

            migrationBuilder.RenameColumn(
                name: "SideType",
                table: "BaseTrades",
                newName: "SideDirection");

            migrationBuilder.AddColumn<bool>(
                name: "IsGreenCandle",
                table: "SRS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInOverNightRange",
                table: "SRS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "BaseTrades",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MaxPrice",
                table: "BaseTrades",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGreenCandle",
                table: "SRS");

            migrationBuilder.DropColumn(
                name: "IsInOverNightRange",
                table: "SRS");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "BaseTrades");

            migrationBuilder.DropColumn(
                name: "MaxPrice",
                table: "BaseTrades");

            migrationBuilder.RenameColumn(
                name: "SideDirection",
                table: "BaseTrades",
                newName: "SideType");

            migrationBuilder.AddColumn<string>(
                name: "Targets",
                table: "BaseTrades",
                type: "NVARCHAR(MAX)",
                nullable: true);
        }
    }
}
