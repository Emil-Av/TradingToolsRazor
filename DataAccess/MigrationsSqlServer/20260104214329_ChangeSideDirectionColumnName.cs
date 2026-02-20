using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSideDirectionColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Direction",
                table: "ResearchCandleBracketing");

            migrationBuilder.RenameColumn(
                name: "SideDirection",
                table: "BaseTrades",
                newName: "Direction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Direction",
                table: "BaseTrades",
                newName: "SideDirection");

            migrationBuilder.AddColumn<int>(
                name: "Direction",
                table: "ResearchCandleBracketing",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
