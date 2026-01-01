using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class TradeRepository : Repository<Trade>, ITradeRepository
    {
        private ApplicationDbContext _db;

        public TradeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(Trade trade)
        {
            Trade? objFromDb = await _db.Trades.FindAsync(trade.Id);
            if (objFromDb is not null)
            {
                var sampleSizeId = objFromDb.SampleSizeId;
                _db.Entry(objFromDb).CurrentValues.SetValues(trade);
                objFromDb.SampleSizeId = sampleSizeId;
            }
        }
    }
}
