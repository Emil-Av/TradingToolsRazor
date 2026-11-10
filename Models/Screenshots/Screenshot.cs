namespace Models.Screenshots
{
    /// <summary>
    /// One screenshot of a trade: who it belongs to, where it is in the trade's carousel and its file name. The image
    /// itself is in <see cref="ScreenshotContent"/> (same Id), so loading trades never loads image bytes.
    /// It belongs to exactly one trade: a trade of the journal (<see cref="BaseTradeId"/>) or a Tom trade
    /// (<see cref="TomTradeId"/>); deleting the trade deletes it and its content.
    /// </summary>
    public class Screenshot
    {
        public int Id { get; set; }

        public int? BaseTradeId { get; set; }

        public int? TomTradeId { get; set; }

        /// <summary>The order in the carousel, from 0; unique per trade. Gaps are left where screenshots were deleted.</summary>
        public int Position { get; set; }

        /// <summary>The file name it was uploaded with, e.g. "DE40_2026-10-27_10-19-54.png"; also gives its image type.</summary>
        public string FileName { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// For screenshots moved from the Screenshots folder into the database: the path exactly as the trade had it
        /// ("Screenshots/Demo Trading/SRS/15M/Sample Size 3/Trade 7/chart.png"). Null for screenshots uploaded since.
        /// </summary>
        public string? LegacyPath { get; set; }

        /// <summary>The image. Only loaded to serve it - never include it when loading trades.</summary>
        public ScreenshotContent? Content { get; set; }
    }

    /// <summary>The bytes of a screenshot's image. Its Id is the Id of its <see cref="Screenshot"/>.</summary>
    public class ScreenshotContent
    {
        public int Id { get; set; }

        public byte[] Data { get; set; } = [];
    }

    /// <summary>
    /// The one-time move of the screenshots from the Screenshots folder into the database: when it ran and what it
    /// found. Its row is the sign that the move is done, so it never runs twice (it runs again after importing an
    /// older dump, which has no row).
    /// </summary>
    public class ScreenshotImport
    {
        public int Id { get; set; }

        public DateTime ImportedAt { get; set; }

        /// <summary>Screenshots copied into the database.</summary>
        public int Imported { get; set; }

        /// <summary>Paths of trades whose file was not found (or lay outside the Screenshots folder); nothing was copied for them.</summary>
        public int Missing { get; set; }
    }
}
