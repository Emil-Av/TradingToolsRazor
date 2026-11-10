using System.Text.Json;
using Shared.Enums;
using SharedEnums.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Trades
{
    public class BaseTrade
    {
        public BaseTrade()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; set; }

        public DateOnly Date { get; set; }
        public string? Symbol { get; set; }

        public double? EntryPrice { get; set; }

        public double? StopPrice { get; set; }

        public double? ExitPrice { get; set; }

        public double? MaxPrice { get; set; }

        public double? Amount { get; set; }

        public double? PnL { get; set; }

        public EStatus Status { get; set; }

        public EDirection Direction { get; set; }

        public ETradeRating TradeRating { get; set; }

        public EOutcome Outcome { get; set; }   

        /// <summary>
        /// The old screenshot paths under wwwroot, from before the screenshots moved into the database. Kept as they
        /// were; no longer written.
        /// </summary>
        public List<string>? ScreenshotsUrls { get; set; }

        /// <summary>The screenshots (without their images); in carousel order once sorted by Position.</summary>
        public List<Models.Screenshots.Screenshot> Screenshots { get; set; } = new();

        public DateTime CreatedAt { get; set; }

        public int SampleSizeId { get; set; }

        [ForeignKey(nameof(SampleSizeId))]
        public SampleSize? SampleSize { get; set; }

        public int? JournalId { get; set; }

        [ForeignKey(nameof(JournalId))]
        public Journal? Journal { get; set; }

        /// <summary>Positions added while the trade was open, in the order they were added (by Id).</summary>
        public List<TradeAddOn> AddOns { get; set; } = new();
    }
}
