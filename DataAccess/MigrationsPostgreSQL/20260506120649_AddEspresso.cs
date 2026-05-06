using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.MigrationsPostgreSQL
{
    /// <inheritdoc />
    public partial class AddEspresso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Espresso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    CandleType = table.Column<int>(type: "integer", nullable: false),
                    IsInOverNightRange = table.Column<bool>(type: "boolean", nullable: false),
                    IsFlippedTheSwitch = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Espresso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Espresso_BaseTrades_Id",
                        column: x => x.Id,
                        principalTable: "BaseTrades",
                        principalColumn: "Id");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Espresso");
        }
    }
}
