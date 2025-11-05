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
    public class UserSettingsRepository : Repository<UserSettings>, IUserSettingsRepository
    {
        private ApplicationDbContext _db;

        public UserSettingsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(UserSettings userSettings)
        {
            UserSettings? objFromDb = await _db.UserSettings.FindAsync(userSettings.Id);
            if (objFromDb != null)
            {
                objFromDb.PTStrategy = userSettings.PTStrategy;
                objFromDb.PTTimeFrame = userSettings.PTTimeFrame;
            }
        }
    }
}
