using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statistics.Models
{
    public class CandleBracketingModel
    {
        public DateOnly Date { get; set; } 

        public bool IsWeekend => Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday;

        public TimeOnly Time { get; set; }

        public EDirection Direction { get; set; }

        /// <summary>
        ///  Relevant to winners only.
        /// </summary>
        public double MaxPrice { get; set; }

        public double EntryPrice { get; set; }

        /// <summary>
        ///  BE and losses only.
        /// </summary>
        public double ExitPrice { get; set; }

        public double CandleHigh { get; set; } 

        public double CandleLow { get; set; }

        public bool IsLoss { get; set; }

        public bool IsWin => !IsLoss && ExitPrice == 0;

        public bool IsBreakeven => !IsLoss && ExitPrice > 0;

        /// <summary>
        /// For Longs.
        /// </summary>
        public double LowestPointAfterEntry { get; set; }

        /// <summary>
        /// For Shorts.
        /// </summary>
        public double HighestPointAfterEntry { get; set; }

        public int ATR { get; set; }

        public ECandleType CandleType { get; set; }

        public bool FlippedTheSwitch { get; set; }  

        public ETimeFrame TimeFrame { get; set; }
    }
}
