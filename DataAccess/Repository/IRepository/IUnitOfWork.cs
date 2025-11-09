using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ITradeRepository Trade { get; }

        IJournalRepository Journal { get; }

        IReviewRepository Review { get; }

        ISampleSizeRepository SampleSize { get; }

        IUserSettingsRepository UserSettings { get; }

        IResearchFirstBarPullbackRepository ResearchFirstBarPullback { get; }

        IResearchCradleRepository ResearchCradle { get; }

        IResearchCandleBracketingRepository CandleBracketing { get; }

        Task SaveAsync();
    }
}
