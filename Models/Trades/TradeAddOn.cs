namespace Models.Trades
{
    /// <summary>
    /// A position added to a trade while it's open (e.g. bought at 1000, then again at 1010 and 1020).
    /// It shares the trade's symbol, direction and date; the max price only applies to the original
    /// trade. Like the trade's own P&amp;L, <see cref="PnL"/> is the number of points as a positive
    /// number (|exit - entry|).
    /// </summary>
    public class TradeAddOn
    {
        public TradeAddOn()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; set; }

        public int BaseTradeId { get; set; }

        public double? EntryPrice { get; set; }

        public double? ExitPrice { get; set; }

        public double? Volume { get; set; }

        public double? PnL { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
