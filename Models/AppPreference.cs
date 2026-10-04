using SharedEnums.Enums;

namespace Models
{
    /// <summary>
    /// App-wide preferences, kept in a single row. For now: which account the app works with by default - the
    /// Dashboard, the History, the Data check and the Trades pages all show that account (Demo Trading or Trade,
    /// i.e. real). No row means the demo account.
    /// </summary>
    public class AppPreference
    {
        public int Id { get; set; }

        public SampleSizeType DefaultAccount { get; set; }
    }
}
