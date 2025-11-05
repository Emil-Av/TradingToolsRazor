using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IUserSettingsRepository : IRepository<UserSettings>
    {
        Task UpdateAsync(UserSettings userSettings);
    }
}
