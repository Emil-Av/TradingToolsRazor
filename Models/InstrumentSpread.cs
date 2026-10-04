namespace Models
{
    /// <summary>
    /// The spread of one instrument, in points: what opening a position costs. It is taken off the result of
    /// every position of a trade in that instrument (the main position and each add-on). Zero or no row means
    /// no spread.
    /// </summary>
    public class InstrumentSpread
    {
        public int Id { get; set; }

        /// <summary>The instrument as stored on trades (the ESymbol name, e.g. "DAX").</summary>
        public string Symbol { get; set; } = string.Empty;

        public double Spread { get; set; }
    }
}
