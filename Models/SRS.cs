using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SRS : BaseTrade
    {
        public bool IsGreenCandle { get; set; }

        public bool IsInOverNightRange { get; set; }
    }
}
