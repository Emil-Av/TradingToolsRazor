using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestModels
{
    public class GetStatisticsModel
    {
        public ETimeFrame TimeFrame { get; set; }

        public EStrategy Strategy { get; set; }

        public ETradeType TradeType { get; set; }

        public TimeOnly? Time { get; set; }
    }
}
