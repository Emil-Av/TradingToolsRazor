using SharedEnums.Enums;

namespace Models
{
    /// <summary>
    /// The account amount of one account (Trade, Demo Trading, ...) in euro: where its equity curve starts. It
    /// used to be a fixed 2000. A reset of the equity curve (<see cref="EquityReset"/>) starts a new curve from
    /// the amount entered then.
    /// </summary>
    public class AccountSetting
    {
        public int Id { get; set; }

        public SampleSizeType AccountType { get; set; }

        public double AccountAmount { get; set; }
    }
}
