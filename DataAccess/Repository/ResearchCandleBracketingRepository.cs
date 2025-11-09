using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ResearchCandleBracketingRepository : Repository<ResearchCandleBracketing>, IResearchCandleBracketingRepository
    {
        private ApplicationDbContext _db;

        public ResearchCandleBracketingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task UpdateAsync(ResearchCandleBracketing researchCandleBracketing)
        {
            ResearchCandleBracketing? objFromDb = await _db.ResearchCandleBracketing.FindAsync(researchCandleBracketing.Id);
            if (objFromDb != null)
            {
                Type type = typeof(ResearchCradle);

                foreach (var property in type.GetProperties())
                {
                    if (property.Name == "Id")
                    {
                        continue;
                    }
                    var value = property.GetValue(researchCandleBracketing);
                    try
                    {
                        property.SetValue(objFromDb, value);
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception as needed, e.g., log it
                        throw new InvalidOperationException($"Error setting property {property.Name}: {ex.Message}", ex);
                    }
                }
            }
        }

        public void UpdateRange(IEnumerable<ResearchCandleBracketing> entities)
        {
            _db.ResearchCandleBracketing.UpdateRange(entities);
        }
    }
}
