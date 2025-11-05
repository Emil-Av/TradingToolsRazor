using Shared.Enums;
using SharedEnums.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.ViewModels.DisplayClasses
{
    public class TradeDisplay
    {
        public TradeDisplay()
        {
            ScreenshotsUrls = new();
        }
        public string? IdDisplay { get; set; }
        public string? SymbolDisplay { get; set; }

        public string? TriggerPriceDisplay { get; set; }

        public string? EntryPriceDisplay { get; set; }

        public string? StopPriceDisplay { get; set; }

        public string? ExitPriceDisplay { get; set; }

        public string? TargetsDisplay { get; set; }

        public string? PnLDisplay { get; set; }

        public string? FeeDisplay { get; set; }

        public EStatus StatusDisplay { get; set; }

        public ESideType? SideTypeDisplay { get; set; }

        public EOrderType? OrderTypeDisplay { get; set; }

        public ETradeRating? TradeRatingDisplay { get; set; }

        public EOutcome? OutcomeDisplay { get; set; }

        public List<string>? ScreenshotsUrls { get; set; }

        public int? SampleSizeId { get; set; }
        [ForeignKey(nameof(SampleSizeId))]

        public SampleSize? SampleSize { get; set; }

        public string? TradeDurationInCandlesDisplay { get; set; }
    }
}
