using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddBrunchBreakStrategy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrunchBreak",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CandleType = table.Column<int>(type: "int", nullable: false),
                    IsFlippedTheSwitch = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrunchBreak", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrunchBreak_BaseTrades_Id",
                        column: x => x.Id,
                        principalTable: "BaseTrades",
                        principalColumn: "Id");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrunchBreak");
        }
    }
}
