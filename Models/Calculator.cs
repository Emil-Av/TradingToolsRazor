using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    ///  Class for calculating the trade price entries, exits, targets and quantity.
    /// </summary>
    public class Calculator
    {
        public double AccountSize { get; set; }

        public double TradeRisk { get; set; }

        public double ExchSizeLimit { get; set; }

        public double MaxSlippage { get; set; }

        public double TradeFee { get; set; }

        public string Symbol { get; set; }

        public double ScaleOut { get; set; }

        public double Precision { get; set; }

        public double Entry { get;set; }

        public double Limit { get; set; }

        public double Stop { get; set; }

        public double EntrySize { get; set; }

        public double EntryQty { get; set; }

        public double OneToOneProfit { get; set; }

        /// <summary>
        ///  Position Profit Percentage
        /// </summary>
        public double PosPPerc {  get; set; }   

        /// <summary>
        ///  Account Profit Percentage
        /// </summary>
        public double AccPPerc { get; set; }

        public double BestTarget { get; set; }

        /// <summary>
        ///  One Cancels Other Quantity (1:1 profit taking quantity)
        /// </summary>
        public double OCOQty { get; set; }

        public double Loss { get; set; }

        /// <summary>
        ///  Position Loss Percentage
        /// </summary>
        public double PosLPerc { get; set; }

        /// <summary>
        ///  Account Loss Percentage
        /// </summary>
        public double AccLPerc { get;set; }

        /// <summary>
        ///  First target depends on the actual entry price (max slippage)
        /// </summary>
        public double WorstTarget { get; set; }

        /// <summary>
        ///  Quantity if the stop is hit
        /// </summary>
        public double StopQty { get; set; }
    }
}
