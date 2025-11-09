using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixTpTMappingForCandleBracketing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove FK that may block table operations
            migrationBuilder.DropForeignKey(
                name: "FK_ResearchFirstBarPullbacks_BaseTrades_Id",
                table: "ResearchFirstBarPullbacks");

            // Drop the existing derived table — recreating it without IDENTITY on Id
            migrationBuilder.DropTable(
                name: "ResearchCandleBracketing");

            migrationBuilder.CreateTable(
                name: "ResearchCandleBracketing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    TimeFrame = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    CandleHigh = table.Column<double>(type: "float", nullable: false),
                    CandleLow = table.Column<double>(type: "float", nullable: false),
                    MaxPrice = table.Column<double>(type: "float", nullable: false),
                    ExitPriceForResearch = table.Column<double>(type: "float", nullable: false),
                    IsLoss = table.Column<bool>(type: "bit", nullable: false),
                    LowestPointAfterEntry = table.Column<double>(type: "float", nullable: false),
                    HighestPointAfterEntry = table.Column<double>(type: "float", nullable: false),
                    ATR = table.Column<int>(type: "int", nullable: false),
                    CandleType = table.Column<int>(type: "int", nullable: false),
                    IsFlippedTheSwitch = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchCandleBracketing", x => x.Id);
                });

            // Create TPT foreign key: ResearchCandleBracketing.Id -> BaseTrades.Id
            migrationBuilder.AddForeignKey(
                name: "FK_ResearchCandleBracketing_BaseTrades_Id",
                table: "ResearchCandleBracketing",
                column: "Id",
                principalTable: "BaseTrades",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            // Restore FK for ResearchFirstBarPullbacks -> BaseTrades (was present previously)
            migrationBuilder.AddForeignKey(
                name: "FK_ResearchFirstBarPullbacks_BaseTrades_Id",
                table: "ResearchFirstBarPullbacks",
                column: "Id",
                principalTable: "BaseTrades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove FKs
            migrationBuilder.DropForeignKey(
                name: "FK_ResearchCandleBracketing_BaseTrades_Id",
                table: "ResearchCandleBracketing");

            migrationBuilder.DropForeignKey(
                name: "FK_ResearchFirstBarPullbacks_BaseTrades_Id",
                table: "ResearchFirstBarPullbacks");

            // Drop the non-identity table and recreate the previous identity-enabled table
            migrationBuilder.DropTable(
                name: "ResearchCandleBracketing");

            migrationBuilder.CreateTable(
                name: "ResearchCandleBracketing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    TimeFrame = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    CandleHigh = table.Column<double>(type: "float", nullable: false),
                    CandleLow = table.Column<double>(type: "float", nullable: false),
                    MaxPrice = table.Column<double>(type: "float", nullable: false),
                    ExitPriceForResearch = table.Column<double>(type: "float", nullable: false),
                    IsLoss = table.Column<bool>(type: "bit", nullable: false),
                    LowestPointAfterEntry = table.Column<double>(type: "float", nullable: false),
                    HighestPointAfterEntry = table.Column<double>(type: "float", nullable: false),
                    ATR = table.Column<int>(type: "int", nullable: false),
                    CandleType = table.Column<int>(type: "int", nullable: false),
                    IsFlippedTheSwitch = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchCandleBracketing", x => x.Id);
                });

            // Recreate FK for ResearchFirstBarPullbacks -> BaseTrades (original behaviour)
            migrationBuilder.AddForeignKey(
                name: "FK_ResearchFirstBarPullbacks_BaseTrades_Id",
                table: "ResearchFirstBarPullbacks",
                column: "Id",
                principalTable: "BaseTrades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
