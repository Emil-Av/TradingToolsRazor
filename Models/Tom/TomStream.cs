namespace Models.Tom
{
    /// <summary>
    /// A YouTube stream (or video) of Tom and Andi: the link, the day it was on, a short title, general notes for the
    /// whole stream, and the trades they took in it. Streams are grouped by month, not by sample size.
    /// </summary>
    public class TomStream
    {
        public TomStream()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; set; }

        public DateOnly Date { get; set; }

        /// <summary>The link to the video.</summary>
        public string? Url { get; set; }

        /// <summary>The session of the stream; null for a stream added before sessions were recorded.</summary>
        public TomSession? Session { get; set; }

        /// <summary>Who trades in it; null for a stream added before traders were recorded.</summary>
        public TomTrader? Trader { get; set; }

        /// <summary>Replaced by <see cref="Session"/> and <see cref="Trader"/>: no longer edited, kept so nothing typed before is lost.</summary>
        public string? Title { get; set; }

        /// <summary>General notes for the whole stream (HTML from the rich text editor), shown with every trade.</summary>
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<TomTrade> Trades { get; set; } = [];
    }
}
