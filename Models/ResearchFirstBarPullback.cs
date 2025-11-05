using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedEnums.Enums;

namespace Models
{
    public class ResearchFirstBarPullback : BaseTrade
    {
        /// <summary>
        ///  Risk to reward ratio 1:1.
        /// </summary>

        public int OneToOneHitOn { get; set; }

        /// <summary>
        ///  Risk to reward ratio 1:3.
        /// </summary>

        public bool IsOneToThreeHit { get; set; }

        /// <summary>
        ///  Risk to reward ratio 1:5.
        /// </summary>

        public bool IsOneToFiveHit { get; set; }

        public bool IsBreakeven { get; set; }

        /// <summary>
        ///  Maximum risk to reward ratio. The number is always in ratio 1:MaxRR (e.g. 1:3)
        /// </summary>
        public int MaxRR { get; set; }

        /// <summary>
        ///  Market gave something - the trade was a loss but it moved in my favor before hitting the stop.
        /// </summary>
        public bool MarketGaveSmth { get; set; }

        public bool IsEntryAfter3To5Bars { get; set; }

        public bool IsEntryAfter5Bars { get; set; }

        public bool IsEntryAtPreviousSwingOnTrigger { get; set; }

        public bool IsEntryBeforePreviousSwingOnTrigger { get; set; }

        public bool IsEntryBeforePreviousSwingOn4H { get; set; }

        public bool IsEntryBeforePreviousSwingOnD { get; set; }

        public bool IsMomentumTrade { get; set; }

        public bool IsTrendTrade { get; set; }

        public bool IsTriggerTrending { get; set; }

        public bool Is4HTrending { get; set; }

        public bool IsDTrending { get; set; }

        public bool IsEntryAfteriBar { get; set; }

        public bool IsSignalBarStrongReversal { get; set; }

        public bool IsSignalBarInTradeDirection { get; set; }

        #region Full ATR
        public int FullATROneToOneHitOn { get; set; }

        public bool IsFullATROneToThreeHit { get; set; }

        public bool IsFullATROneToFiveHit { get; set; }

        public bool IsFullATRBreakeven { get; set; }

        public bool IsFullATRLoss { get; set; }

        public int FullATRMaxRR { get; set; }

        public bool FullATRMarketGaveSmth { get; set; }

        #endregion

        public string? Comment { get; set; }

    }
}
