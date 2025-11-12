using DataAccess.Data;
using DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public ITradeRepository Trade { get; private set; }

        public IReviewRepository Review { get; private set; }

        public IJournalRepository Journal { get; private set; }

        public ISampleSizeRepository SampleSize { get; private set; }

        public IUserSettingsRepository UserSettings { get; private set; }

        public IResearchFirstBarPullbackRepository ResearchFirstBarPullback { get; private set; }

        public IResearchCradleRepository ResearchCradle { get; private set; }

        public IResearchCandleBracketingRepository ResearchCandleBracketing { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Trade = new TradeRepository(_db);
            Review = new ReviewRepository(_db);
            Journal = new JournalRepository(_db);
            SampleSize = new SampleSizeRepository(_db);
            UserSettings = new UserSettingsRepository(_db);
            ResearchFirstBarPullback = new ResearchFirstBarPullbackRepository(_db);
            ResearchCradle = new ResearchCradleRepository(_db);
            ResearchCandleBracketing = new ResearchCandleBracketingRepository(_db);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
