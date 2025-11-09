using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IResearchCandleBracketingRepository : IRepository<ResearchCandleBracketing>
    {
        Task UpdateAsync(ResearchCandleBracketing objFromDb);
        void UpdateRange(IEnumerable<ResearchCandleBracketing> entities);
    }
}
