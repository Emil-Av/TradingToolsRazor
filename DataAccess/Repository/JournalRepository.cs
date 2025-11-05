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
    public class JournalRepository : Repository<Journal>, IJournalRepository
    {
        private ApplicationDbContext _db;

        public JournalRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public async Task UpdateAsync(Journal journal)
        {
            Journal? objFromDb = await _db.Journals.FindAsync(journal.Id);
            if (objFromDb != null)
            {
                objFromDb.Pre = journal.Pre;
                objFromDb.During = journal.During;
                objFromDb.Exit = journal.Exit;
                objFromDb.Post = journal.Post;
            }
        }
    }
}
