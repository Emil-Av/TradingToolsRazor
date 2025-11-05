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

        public async Task UpdateAsync(Trade paperTrade)
        {
            Trade? objFromDb = await _db.Trades.FindAsync(paperTrade.Id);
            if (objFromDb != null)
            {
                objFromDb.SampleSize = paperTrade.SampleSize;
                objFromDb.Symbol = paperTrade.Symbol;
                objFromDb.SideType = paperTrade.SideType;
                objFromDb.Status = paperTrade.Status;
                objFromDb.Outcome = paperTrade.Outcome;
                objFromDb.TradeRating = paperTrade.TradeRating;
                objFromDb.TriggerPrice = paperTrade.TriggerPrice;
                objFromDb.EntryPrice = paperTrade.EntryPrice;
                objFromDb.StopPrice = paperTrade.StopPrice;
                objFromDb.ExitPrice = paperTrade.ExitPrice;
                objFromDb.Targets = paperTrade.Targets;
                objFromDb.PnL = paperTrade.PnL;
                objFromDb.Fee = paperTrade.Fee;
                objFromDb.OrderType = paperTrade.OrderType;
                objFromDb.ScreenshotsUrls = paperTrade.ScreenshotsUrls;
                objFromDb.SampleSize.TradeType = paperTrade.SampleSize.TradeType;
            }
        }
    }
}
