using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IBaseTradeRepository BaseTrade { get; }

        IJournalRepository Journal { get; }

        IReviewRepository Review { get; }

        ISampleSizeRepository SampleSize { get; }

        IUserSettingsRepository UserSettings { get; }

        IResearchFirstBarPullbackRepository ResearchFirstBarPullback { get; }

        IResearchCradleRepository ResearchCradle { get; }

        IResearchCandleBracketingRepository ResearchCandleBracketing { get; }

        ISRSRepository SRS { get; }

        IBrunchBreakRepository BrunchBreak { get; }

        Task SaveAsync();
    }
}
