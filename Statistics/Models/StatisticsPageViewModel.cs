using SharedEnums.Enums;
using Models;
using Statistics.Models;
using System.Collections.Generic;
using Models.Trades;

namespace Statistics.Models
{
    public class StatisticsPageViewModel
    {
        public List<TimeFrame> AvailableTimeFrames { get; set; } = [];
        public List<Strategy> AvailableStrategies { get; set; } = [];

        public SampleSize? CurrentSampleSize { get; set; }
        public int NumberSampleSizes { get; set; }
        public int CurrentSampleSizeNumber { get; set; }

        public List<TimeOnly> Times { get; set; } = [];

        public List<CandleBracketingStatisticItem> CandleBracketingStats { get; set; } = [];
        public ResearchCandleBracketing? CurrentResearchCandleBracketingTrade { get; set; }
    }
}
