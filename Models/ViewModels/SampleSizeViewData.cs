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
        public TimeFrame TimeFrame { get; set; }

        public Strategy Strategy { get; set; }

        public TradeType TradeType { get; set; }
    }
}
