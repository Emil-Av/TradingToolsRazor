using System.ComponentModel.DataAnnotations.Schema;
using Shared.Enums;
using SharedEnums.Enums;

namespace Models.Trades
{
    public class ResearchCandleBracketing : BaseTrade
    {
        public DateOnly Date { get; set; }

        public TimeOnly Time { get;set; }

        public EDirection Direction { get; set; }

        public double CandleHigh { get; set; }

        public double CandleLow { get; set; }   

        public double EntryPriceForResearch { get; set; }

        /// <summary>
        ///  Relevant for BE and losses only.
        /// </summary>
        public double ExitPriceForResearch { get; set; }

        [NotMapped]
        public bool IsWin => !IsLoss && ExitPriceForResearch == 0;

        public bool IsLoss { get; set; }

        [NotMapped]
        public bool IsBreakeven => !IsLoss && ExitPriceForResearch > 0;

        /// <summary>
        /// For longs.
        /// </summary>
        public double LowestPointAfterEntry { get; set; }

        /// <summary>
        /// For shorts.
        /// </summary>
        public double HighestPointAfterEntry { get; set; }

        public int ATR { get; set; }

        public bool IsWeekend { get; set; }

        public ECandleType CandleType { get; set; }

        public bool IsFlippedTheSwitch { get; set; }
    }
}
