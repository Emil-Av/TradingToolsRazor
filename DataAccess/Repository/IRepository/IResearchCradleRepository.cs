using Models.Trades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IResearchCradleRepository : IRepository<ResearchCradle>
    {
        Task UpdateAsync(ResearchCradle objFromDb);

        void UpdateRange(IEnumerable<ResearchCradle> entities);
    }
}
