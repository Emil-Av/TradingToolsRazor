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
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private ApplicationDbContext _db;

        public ReviewRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public async Task UpdateAsync(Review review)
        {
            Review? objFromDb = await _db.Reviews.FindAsync(review.Id);
            if (objFromDb != null)
            {
                objFromDb.First = review.First;
                objFromDb.Second = review.Second;
                objFromDb.Third = review.Third;
                objFromDb.Forth = review.Forth;
                objFromDb.Summary = review.Summary;
            }
        }
    }
}
