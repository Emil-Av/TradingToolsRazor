using Shared.Enums;
using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ResearchCandleBracketing : BaseTrade
    {
        public DateOnly Date { get; set; }

        public TimeOnly Time { get;set; }

        public ETimeFrame TimeFrame { get; set; }   

        public EDirection Direction { get; set; }

        public double CandleHigh { get; set; }

        public double CandleLow { get; set; }   

        public double MaxPrice { get; set; }

        /// <summary>
        ///  Relevant for BE and losses only.
        /// </summary>
        public new double ExitPrice { get; set; }

        public bool IsLoss { get; set; }

        /// <summary>
        /// For longs.
        /// </summary>
        public double LowestPointAfterEntry { get; set; }

        /// <summary>
        /// For shorts.
        /// </summary>
        public double HighestPointAfterEntry { get; set; }

        public int ATR { get; set; }

        public bool IsWeekend => Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday;

        public ECandleType CandleType { get; set; }

        public bool IsFlippedTheSwitch { get; set; }
    }
}
