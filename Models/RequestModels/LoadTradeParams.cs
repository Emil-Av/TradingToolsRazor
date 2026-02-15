using Shared;
using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestModels
{
    public class LoadTradeParams
    {
        public void ConvertParamsFromView()
        {
            Result<EStatus> resultStatus = MyEnumConverter.StatusFromString(StatusFromView);
            Result<TimeFrame> resultTimeFrame = MyEnumConverter.TimeFrameFromString(TimeFrameFromView);
            Result<Strategy> resultStrategy = MyEnumConverter.StrategyFromString(StrategyFromView);
            Result<SampleSizeType> resultTradeType = MyEnumConverter.TradeTypeFromString(TradeTypeFromView);

            if (resultStatus.Success)
            {
                Status = resultStatus.Value;
            }
            if (resultTimeFrame.Success)
            {
                TimeFrame = resultTimeFrame.Value;
            }
            if (resultStrategy.Success)
            {
                Strategy = resultStrategy.Value;
            }
            if (resultTradeType.Success)
            {
                SampleSizeType = resultTradeType.Value;
            }

            SampleSizeNumber = int.Parse(SampleSizeNumberFromView);
            TradeNumber = int.Parse(TradeNumberFromView);
            ShowLastTrade = bool.Parse(ShowLastTradeFromView);
            LoadLastSampleSize = bool.Parse(LoadLastSampleSizeFromView);
            StatusChanged = bool.Parse(StatusChangedFromView);
        }

        #region Values from the view
        public string StatusFromView { get; set; }

        public string TimeFrameFromView { get; set; }

        public string StrategyFromView { get; set; }

        public string TradeTypeFromView { get; set; }

        public string SampleSizeNumberFromView { get; set; }

        public string TradeNumberFromView { get; set; }

        public string ShowLastTradeFromView { get; set; }

        public string LoadLastSampleSizeFromView { get; set; }

        public string StatusChangedFromView { get; set; }

        #endregion

        #region Properties for after conversion

        public bool StatusChanged { get;set; }

        public bool LoadLastSampleSize { get; set; }

        public bool ShowLastTrade { get; set; }

        public int TradeNumber { get; set; }

        public int SampleSizeNumber { get; set; }

        public EStatus Status { get; set; }

        public TimeFrame TimeFrame { get; set; }

        public Strategy Strategy { get; set; }

        public SampleSizeType SampleSizeType { get; set; }

        #endregion
    }
}
