using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestModels
{
    public class StatisticsQueryModel
    {
        public ETimeFrame? TimeFrame { get; set; }

        public EStrategy? Strategy { get; set; }

        public ETradeType? TradeType { get; set; }

        /// <summary>
        /// True if all nullable props are null.
        /// </summary>
        public bool IsInitialLoading => TimeFrame == null;

        public int SampleSizeNumber { get; set; }

        public string? Time { get; set; }
    }
}
