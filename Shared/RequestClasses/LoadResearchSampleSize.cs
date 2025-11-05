using SharedEnums.Enums;
using System.Reflection.Metadata;

namespace Shared
{
    public class LoadResearchSampleSize
    {
        public string TimeFrame { get; set; }

        public string Strategy { get; set; }

        public string SampleSizeNumber { get; set; }

        public string IsSampleSizeChanged { get; set; } 

        public string IsTimeFrameChanged { get; set; }

        public string IsStrategyChanged { get; set; }
    }
}
