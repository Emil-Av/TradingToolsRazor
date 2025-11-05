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
    public class SampleSizeRepository : Repository<SampleSize>, ISampleSizeRepository
    {
        private ApplicationDbContext _db;

        public SampleSizeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(SampleSize sampleSize)
        {

        }
    }
}
