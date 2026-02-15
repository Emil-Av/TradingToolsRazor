using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Trades;
using Models.ViewModels.DisplayClasses;
using SharedEnums.Enums;

namespace Models.ViewModels
{
    public class TradesVM
    {
        public TradesVM()
        {
            AvailableStrategies = [];
            AvailableTimeframes = [];
            TradeRatingOptions =
            [
                new SelectListItem { Value = "0", Text = "A+"},
                new SelectListItem { Value = "1", Text = "A"},
                new SelectListItem { Value = "2", Text = "A-" },
                new SelectListItem { Value = "3", Text = "Book of Horror" }
            ];
            TradeData = new();
            CurrentTrade = new();
            SRSTrade = new();
            BrunchBreakTrade = new();
            CurrentSampleSize = new();
            AllTradesInSampleSize = [];
            SampleSizes = [];
        }

        // The number of sample sizes for a strategy and time frame
        public int NumberSampleSizes { get; set; }

        public int CurrentSampleSizeNumber { get; set; }

        public string? ErrorMsg { get; set; }

        // The trade being displayed
        public BaseTrade CurrentTrade { get; set; }

        public SRS SRSTrade { get; set; }

        public BrunchBreak BrunchBreakTrade { get; set; }

        public TradeDisplay TradeData { get; set; }

        public SampleSize CurrentSampleSize { get; set; }

        public List<SampleSize> SampleSizes { get; set; }

        // All trades in the current sample size (for client-side navigation)
        public List<object> AllTradesInSampleSize { get; set; }

        // The current number of trades for the latest sample size
        public List<Strategy> AvailableStrategies { get; set; }

        public List<TimeFrame> AvailableTimeframes { get; set; }

        public List<SelectListItem> TradeRatingOptions { get; set; }

        public EStatus DefaultTradeStatus { get; set; } = EStatus.All;

    }
}
