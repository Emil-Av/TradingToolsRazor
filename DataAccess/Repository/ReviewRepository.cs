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
    public class ReviewRepository(ApplicationDbContext db) : Repository<Review>(db), IReviewRepository
    {
        private readonly ApplicationDbContext _db = db;

        public async Task UpdateAsync(Review review)
        {
            Review? objFromDb = await _db.Reviews.FindAsync(review.Id);
            if (objFromDb is not null)
            {
                _db.Entry(objFromDb).CurrentValues.SetValues(review);
            }
        }
    }
}
