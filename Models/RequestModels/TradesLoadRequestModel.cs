using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestModels
{
    public class TradesLoadRequestModel
    {
        public EStrategy Strategy { get; set; }

        public ETradeType TradeType { get; set; }

        public EStatus Status { get; set; }

        public ETimeFrame TimeFrame { get; set; }

        public int SampleSizeId { get; set; }
    }
}
