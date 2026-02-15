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
    public class SRSRepository(ApplicationDbContext db) : Repository<SRS>(db), ISRSRepository
    {
        private readonly ApplicationDbContext _db = db;

        public async Task UpdateAsync(SRS srs)
        {
            SRS? objFromDb = await _db.SRS.FindAsync(srs.Id);

            if (objFromDb is not null)
            {
                _db.Entry(objFromDb).CurrentValues.SetValues(srs);
            }
        }

        public void UpdateRange(IEnumerable<SRS> entities)
        {
            _db.SRS.UpdateRange(entities);
        }
    }
}
