using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestModels
{
    public class TradesLoadTimeFrameRequestModel
    {
        public Strategy Strategy { get; set; }

        public TradeType TradeType { get; set; }

        public TimeFrame TimeFrame { get; set; }

    }
}
