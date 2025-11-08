using System.Text.Json;
using Shared.Enums;
using SharedEnums.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class BaseTrade
    {
        public BaseTrade()
        {
            CreatedAt = DateTime.Now;
        }

        public int Id { get; set; }
        public string? Symbol { get; set; }

        public double? TriggerPrice { get; set; }

        public double? EntryPrice { get; set; }

        public double? StopPrice { get; set; }

        public double? ExitPrice { get; set; }

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string? Targets { get; set; }

        public double? PnL { get; set; }

        public double? Fee { get; set; }

        public EStatus Status { get; set; }

        public EDirection SideType { get; set; }

        public EOrderType OrderType { get; set; }

        public ETradeRating TradeRating { get; set; }

        public EOutcome Outcome { get; set; }   

        public List<string>? ScreenshotsUrls { get; set; }

        public int SampleSizeId { get; set; }

        [ForeignKey(nameof(SampleSizeId))]
        public SampleSize? SampleSize { get; set; }

        /// <summary>
        ///  All trades except research trades will have a Journal.
        /// </summary>
        public int? JournalId { get; set; }

        [ForeignKey(nameof(JournalId))]
        public Journal? Journal { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
