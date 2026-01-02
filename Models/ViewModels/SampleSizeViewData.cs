using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    /// <summary>
    /// The class represents the values of the menu buttons.
    /// </summary>
    public class SampleSizeViewData
    {
        public ETimeFrame TimeFrame { get; set; }

        public EStrategy Strategy { get; set; }

        public ETradeType TradeType { get; set; }
    }
}
