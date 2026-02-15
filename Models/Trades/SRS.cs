using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Trades
{
    public class SRS : BaseTrade
    {
        public ECandleType CandleType { get; set; }

        public bool IsInOverNightRange { get; set; }

        public bool IsFlippedTheSwitch { get; set; }
    }
}
