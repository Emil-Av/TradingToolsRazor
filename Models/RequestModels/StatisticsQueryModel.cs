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
        public TimeFrame? TimeFrame { get; set; }

        public Strategy? Strategy { get; set; }

        public TradeType? TradeType { get; set; }

        /// <summary>
        /// True if all nullable props are null.
        /// </summary>
        public bool IsInitialLoading => TimeFrame == null;

        public bool IsNewLoad { get; set; }

        public int SampleSizeNumber { get; set; }

        public string? Time { get; set; }
    }
}
