using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.MigrationsSqlServer
{
    /// <inheritdoc />
    public partial class SqlServerInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Journals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    During = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Post = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    First = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Second = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Third = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Forth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PTTimeFrame = table.Column<int>(type: "int", nullable: false),
                    PTStrategy = table.Column<int>(type: "int", nullable: false),
                    AccountSize = table.Column<double>(type: "float", nullable: false),
                    TradeRisk = table.Column<double>(type: "float", nullable: false),
                    ExchSizeLimit = table.Column<double>(type: "float", nullable: false),
                    MaxSlippage = table.Column<double>(type: "float", nullable: false),
                    TradeFee = table.Column<double>(type: "float", nullable: false),
                    ScaleOut = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SampleSizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Strategy = table.Column<int>(type: "int", nullable: false),
                    TimeFrame = table.Column<int>(type: "int", nullable: false),
                    SampleSizeType = table.Column<int>(type: "int", nullable: false),
                    ReviewId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleSizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SampleSizes_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BaseTrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TriggerPrice = table.Column<double>(type: "float", nullable: true),
                    EntryPrice = table.Column<double>(type: "float", nullable: true),
                    StopPrice = table.Column<double>(type: "float", nullable: true),
                    ExitPrice = table.Column<double>(type: "float", nullable: true),
                    MaxPrice = table.Column<double>(type: "float", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: true),
                    PnL = table.Column<double>(type: "float", nullable: true),
                    Fee = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    TradeRating = table.Column<int>(type: "int", nullable: false),
                    Outcome = table.Column<int>(type: "int", nullable: false),
                    ScreenshotsUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SampleSizeId = table.Column<int>(type: "int", nullable: false),
                    JournalId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseTrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseTrades_Journals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "Journals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BaseTrades_SampleSizes_SampleSizeId",
                        column: x => x.SampleSizeId,
                        principalTable: "SampleSizes",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateTable(
                name: "ResearchCandleBracketing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    CandleHigh = table.Column<double>(type: "float", nullable: false),
                    CandleLow = table.Column<double>(type: "float", nullable: false),
                    EntryPriceForResearch = table.Column<double>(type: "float", nullable: false),
                    ExitPriceForResearch = table.Column<double>(type: "float", nullable: false),
                    IsLoss = table.Column<bool>(type: "bit", nullable: false),
                    LowestPointAfterEntry = table.Column<double>(type: "float", nullable: false),
                    HighestPointAfterEntry = table.Column<double>(type: "float", nullable: false),
                    ATR = table.Column<int>(type: "int", nullable: false),
                    IsWeekend = table.Column<bool>(type: "bit", nullable: false),
                    CandleType = table.Column<int>(type: "int", nullable: false),
                    IsFlippedTheSwitch = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchCandleBracketing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResearchCandleBracketing_BaseTrades_Id",
                        column: x => x.Id,
                        principalTable: "BaseTrades",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ResearchCradles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    TestCradleProp = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchCradles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResearchCradles_BaseTrades_Id",
                        column: x => x.Id,
                        principalTable: "BaseTrades",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ResearchFirstBarPullbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    OneToOneHitOn = table.Column<int>(type: "int", nullable: false),
                    IsOneToThreeHit = table.Column<bool>(type: "bit", nullable: false),
                    IsOneToFiveHit = table.Column<bool>(type: "bit", nullable: false),
                    IsBreakeven = table.Column<bool>(type: "bit", nullable: false),
                    MaxRR = table.Column<int>(type: "int", nullable: false),
                    MarketGaveSmth = table.Column<bool>(type: "bit", nullable: false),
                    IsEntryAfter3To5Bars = table.Column<bool>(type: "bit", nullable: false),
                    IsEntryAfter5Bars = table.Column<bool>(type: "bit", nullable: false),
                    IsEntryAtPreviousSwingOnTrigger = table.Column<bool>(type: "bit", nullable: false),
                    IsEntryBeforePreviousSwingOnTrigger = table.Column<bool>(type: "bit", nullable: false),
                    IsEntryBeforePreviousSwingOn4H = table.Column<bool>(type: "bit", nullable: false),
                    IsEntryBeforePreviousSwingOnD = table.Column<bool>(type: "bit", nullable: false),
                    IsMomentumTrade = table.Column<bool>(type: "bit", nullable: false),
                    IsTrendTrade = table.Column<bool>(type: "bit", nullable: false),
                    IsTriggerTrending = table.Column<bool>(type: "bit", nullable: false),
                    Is4HTrending = table.Column<bool>(type: "bit", nullable: false),
                    IsDTrending = table.Column<bool>(type: "bit", nullable: false),
                    IsEntryAfteriBar = table.Column<bool>(type: "bit", nullable: false),
                    IsSignalBarStrongReversal = table.Column<bool>(type: "bit", nullable: false),
                    IsSignalBarInTradeDirection = table.Column<bool>(type: "bit", nullable: false),
                    FullATROneToOneHitOn = table.Column<int>(type: "int", nullable: false),
                    IsFullATROneToThreeHit = table.Column<bool>(type: "bit", nullable: false),
                    IsFullATROneToFiveHit = table.Column<bool>(type: "bit", nullable: false),
                    IsFullATRBreakeven = table.Column<bool>(type: "bit", nullable: false),
                    IsFullATRLoss = table.Column<bool>(type: "bit", nullable: false),
                    FullATRMaxRR = table.Column<int>(type: "int", nullable: false),
                    FullATRMarketGaveSmth = table.Column<bool>(type: "bit", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchFirstBarPullbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResearchFirstBarPullbacks_BaseTrades_Id",
                        column: x => x.Id,
                        principalTable: "BaseTrades",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SRS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CandleType = table.Column<int>(type: "int", nullable: false),
                    IsInOverNightRange = table.Column<bool>(type: "bit", nullable: false),
                    IsFlippedTheSwitch = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SRS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SRS_BaseTrades_Id",
                        column: x => x.Id,
                        principalTable: "BaseTrades",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ResearchId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trades_BaseTrades_Id",
                        column: x => x.Id,
                        principalTable: "BaseTrades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Trades_ResearchFirstBarPullbacks_ResearchId",
                        column: x => x.ResearchId,
                        principalTable: "ResearchFirstBarPullbacks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BaseTrades_JournalId",
                table: "BaseTrades",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseTrades_SampleSizeId",
                table: "BaseTrades",
                column: "SampleSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleSizes_ReviewId",
                table: "SampleSizes",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_ResearchId",
                table: "Trades",
                column: "ResearchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BrunchBreak");

            migrationBuilder.DropTable(
                name: "ResearchCandleBracketing");

            migrationBuilder.DropTable(
                name: "ResearchCradles");

            migrationBuilder.DropTable(
                name: "SRS");

            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ResearchFirstBarPullbacks");

            migrationBuilder.DropTable(
                name: "BaseTrades");

            migrationBuilder.DropTable(
                name: "Journals");

            migrationBuilder.DropTable(
                name: "SampleSizes");

            migrationBuilder.DropTable(
                name: "Reviews");
        }
    }
}
