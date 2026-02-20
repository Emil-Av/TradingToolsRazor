using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data
{
    public class SqlServerDbContext : ApplicationDbContext
    {
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) 
            : base(options)
        {
        }
    }
}
