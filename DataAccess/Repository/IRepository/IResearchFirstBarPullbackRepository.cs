using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IResearchFirstBarPullbackRepository : IRepository<ResearchFirstBarPullback>
    {
        Task UpdateAsync(ResearchFirstBarPullback objFromDb);

        void UpdateRange(IEnumerable<ResearchFirstBarPullback> entities);
    }
}
