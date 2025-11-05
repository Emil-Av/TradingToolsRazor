using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.DisplayClasses
{
    public class ResearchFirstBarPullbackDisplay : TradeDisplay
    {
        /// <summary>
        ///  Class for the ResearchView. Display at the end of the property name indicates that the property should be displayed in the view (some of the        entity properties will not be displayed).
        /// </summary>

        /// <summary>
        ///  Risk to reward ratio 1:1.
        /// </summary>
        [Range(0, 6)]
        public string OneToOneHitOnDisplay { get; set; }

        /// <summary>
        ///  Risk to reward ratio 1:3.
        /// </summary>

        public string IsOneToThreeHitDisplay { get; set; }

        /// <summary>
        ///  Risk to reward ratio 1:5.
        /// </summary>

        public string IsOneToFiveHitDisplay { get; set; }

        public string IsBreakevenDisplay { get; set; }

        /// <summary>
        ///  Maximum risk to reward ratio. The number is always in ratio 1:MaxRR (e.g. 1:3)
        /// </summary>
        public int MaxRRDisplay { get; set; }

        /// <summary>
        ///  Market gave something - the trade was a loss but it moved in my favor before hitting the stop.
        /// </summary>
        public string MarketGaveSmthDisplay { get; set; }

        public string IsEntryAfter3To5BarsDisplay { get; set; }

        public string IsEntryAfter5BarsDisplay { get; set; }

        public string IsEntryAtPreviousSwingOnTriggerDisplay { get; set; }

        public string IsEntryBeforePreviousSwingOnTriggerDisplay { get; set; }

        public string IsEntryBeforePreviousSwingOn4HDisplay { get; set; }

        public string IsEntryBeforePreviousSwingOnDDisplay { get; set; }

        public string IsMomentumTradeDisplay { get; set; }

        public string IsTrendTradeDisplay { get; set; }

        public string IsTriggerTrendingDisplay { get; set; }

        public string Is4HTrendingDisplay { get; set; }

        public string IsDTrendingDisplay { get; set; }

        public string IsEntryAfteriBarDisplay { get; set; }

        public string IsSignalBarStrongReversalDisplay { get; set; }

        public string IsSignalBarInTradeDirectionDisplay { get; set; }

        #region Full ATR
        public string FullATROneToOneHitOnDisplay { get; set; }

        public string IsFullATROneToThreeHitDisplay { get; set; }

        public string IsFullATROneToFiveHitDisplay { get; set; }

        public string IsFullATRBreakevenDisplay { get; set; }

        public string IsFullATRLossDisplay { get; set; }

        public int FullATRMaxRRDisplay { get; set; }
        public string FullATRMarketGaveSmthDisplay { get; set; }

        #endregion

        public string? CommentDisplay { get; set; }
    }
}
