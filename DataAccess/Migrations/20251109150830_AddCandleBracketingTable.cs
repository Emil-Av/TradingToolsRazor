using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCandleBracketingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResearchCandleBracketing");

          
        }
    }
}
