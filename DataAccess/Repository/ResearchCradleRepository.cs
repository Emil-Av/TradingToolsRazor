using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ResearchCradleRepository : Repository<ResearchCradle>, IResearchCradleRepository
    {
        private ApplicationDbContext _db;

        public ResearchCradleRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(ResearchCradle researchCradle)
        {
            ResearchCradle? objFromDb = await _db.ResearchCradles.FindAsync(researchCradle.Id);
            if (objFromDb != null)
            {
                Type type = typeof(ResearchCradle);

                foreach (var property in type.GetProperties())
                {
                    if (property.Name == "Id" || property.Name == "SampleSizeId")
                    {
                        continue;
                    }
                    var value = property.GetValue(researchCradle);
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

        public void UpdateRange(IEnumerable<ResearchCradle> entities)
        {
            _db.ResearchCradles.UpdateRange(entities);
        }
    }
}
