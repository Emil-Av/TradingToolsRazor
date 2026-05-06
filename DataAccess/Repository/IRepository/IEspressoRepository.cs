using Models.Trades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IEspressoRepository : IRepository<Espresso>
    {
        Task UpdateAsync(Espresso objFromDb);

        void UpdateRange(IEnumerable<Espresso> entities);
    }
}
