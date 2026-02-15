using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models.Trades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class BaseTradeRepository(ApplicationDbContext db) : Repository<BaseTrade>(db), IBaseTradeRepository
    {
        private readonly ApplicationDbContext _db = db;

        public async Task UpdateAsync(BaseTrade trade)
        {
            BaseTrade objFromDb = (await _db.BaseTrades.FindAsync(trade.Id))!;
            if (objFromDb is not null)
            {
                _db.Entry(objFromDb).CurrentValues.SetValues(trade);
            }
        }
    }
}
