using SharedEnums.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class SampleSize
    {
        public int Id { get; set; }

        public Strategy Strategy { get; set; }

        public TimeFrame TimeFrame { get; set; }

        public SampleSizeType SampleSizeType { get; set; }

        /// <summary>
        ///  ReviewId is null for research trades.
        /// </summary>
        public int? ReviewId { get; set; }

        [ForeignKey(nameof(ReviewId))]
        public Review? Review { get; set; }
    }
}
