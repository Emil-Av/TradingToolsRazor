using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BrunchBreak : BaseTrade
    {
        public ECandleType CandleType { get; set; }

        public bool IsFlippedTheSwitch { get; set; }
    }
}
