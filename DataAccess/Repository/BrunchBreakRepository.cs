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
    public class BrunchBreakRepository(ApplicationDbContext db) : Repository<BrunchBreak>(db), IBrunchBreakRepository
    {
        private readonly ApplicationDbContext _db = db;
        public async Task UpdateAsync(BrunchBreak brunchBreak)
        {
            BrunchBreak? objFromDb = await _db.BrunchBreak.FindAsync(brunchBreak.Id);

            if (objFromDb is not null)
            {
                _db.Entry(objFromDb).CurrentValues.SetValues(brunchBreak);
            }
        }

        public void UpdateRange(IEnumerable<BrunchBreak> entities)
        {
            _db.BrunchBreak.UpdateRange(entities);
        }
    }
}
