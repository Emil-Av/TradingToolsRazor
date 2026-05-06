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
    public class EspressoRepository : Repository<Espresso>, IEspressoRepository
    {
        private readonly ApplicationDbContext _db;
        public EspressoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task UpdateAsync(Espresso espresso)
        {
            Espresso? objFromDb = await _db.Espresso.FindAsync(espresso.Id);
            if (objFromDb is not null)
            {
                _db.Entry(objFromDb).CurrentValues.SetValues(espresso);
            }
        }
        public void UpdateRange(IEnumerable<Espresso> entities)
        {
            _db.Espresso.UpdateRange(entities);
        }
    }
}
