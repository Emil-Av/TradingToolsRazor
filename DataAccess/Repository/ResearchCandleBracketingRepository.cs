using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models.Trades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ResearchCandleBracketingRepository(ApplicationDbContext db) : Repository<ResearchCandleBracketing>(db), IResearchCandleBracketingRepository
    {
        private readonly ApplicationDbContext _db = db;

        public async Task UpdateAsync(ResearchCandleBracketing researchCandleBracketing)
        {
            ResearchCandleBracketing? objFromDb = await _db.ResearchCandleBracketing.FindAsync(researchCandleBracketing.Id);
            if (objFromDb != null)
            {
                var originalSampleSizeId = objFromDb.SampleSizeId;
                _db.Entry(objFromDb).CurrentValues.SetValues(researchCandleBracketing);
                objFromDb.SampleSizeId = originalSampleSizeId;
            }
        }

        public void UpdateRange(IEnumerable<ResearchCandleBracketing> entities)
        {
            _db.ResearchCandleBracketing.UpdateRange(entities);
        }
    }
}
