using SharedEnums.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class SampleSize
    {
        public SampleSize()
        {
        }
        public int Id { get; set; }

        public EStrategy Strategy { get; set; }

        public ETimeFrame TimeFrame { get; set; }

        public ETradeType TradeType { get; set; }

        /// <summary>
        ///  ReviewId is null for research trades.
        /// </summary>
        public int? ReviewId { get; set; }

        [ForeignKey(nameof(ReviewId))]
        public Review? Review { get; set; }
    }
}
