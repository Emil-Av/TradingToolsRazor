namespace Models.Tom
{
    /// <summary>
    /// A trade Tom or Andi took in a stream: its screenshots and my notes on it. No trade data (prices, size, ...):
    /// it is there to learn from, not to count.
    /// </summary>
    public class TomTrade
    {
        public TomTrade()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; set; }

        public int TomStreamId { get; set; }

        /// <summary>My thoughts and notes on this trade (HTML from the rich text editor).</summary>
        public string? Notes { get; set; }

        /// <summary>Where in the video the trade is, in seconds from the start; null when not set.</summary>
        public int? VideoSeconds { get; set; }

        /// <summary>
        /// The old screenshot paths under wwwroot ("Screenshots/Tom/..."), from before the screenshots moved into the
        /// database. Kept as they were; no longer written.
        /// </summary>
        public List<string>? ScreenshotsUrls { get; set; } = [];

        /// <summary>The screenshots (without their images); in carousel order once sorted by Position.</summary>
        public List<Models.Screenshots.Screenshot> Screenshots { get; set; } = [];

        public DateTime CreatedAt { get; set; }
    }
}
