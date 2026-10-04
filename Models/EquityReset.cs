using SharedEnums.Enums;

namespace Models
{
    /// <summary>
    /// A reset of an account's equity curve: from the next trade on, the curve starts again at
    /// <see cref="StartingBalance"/>. Nothing is deleted - trades stay as they are, and the old curve is still
    /// there in the "All history" view. Trades created after <see cref="ResetAt"/> belong to the new curve.
    /// </summary>
    public class EquityReset
    {
        public int Id { get; set; }

        public SampleSizeType AccountType { get; set; }

        /// <summary>When the reset was made (UTC), compared with the creation time of the trades.</summary>
        public DateTime ResetAt { get; set; }

        /// <summary>The balance the new curve starts at.</summary>
        public double StartingBalance { get; set; }

        /// <summary>The starting balance of the curve this reset ended, kept so the old curve can still be drawn.</summary>
        public double PreviousStartingBalance { get; set; }
    }
}
