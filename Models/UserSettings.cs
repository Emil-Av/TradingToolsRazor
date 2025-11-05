using System.ComponentModel;
using SharedEnums.Enums;

namespace Models
{
    public class UserSettings
    {
        public int Id { get; set; }

        /// <summary>
        ///  Last used time frame in PaperTrades
        /// </summary>

        public ETimeFrame PTTimeFrame { get; set; }

        [DefaultValue(0)]
        /// <summary>
        ///  Last used strategy in PaperTrades
        /// </summary>
        public EStrategy PTStrategy { get; set; }    

        public double AccountSize { get; set; }

        public double TradeRisk { get; set; }

        public double ExchSizeLimit { get; set; }

        public double MaxSlippage { get; set; }

        public double TradeFee { get; set; }

        public double ScaleOut { get; set; }
    }
}
