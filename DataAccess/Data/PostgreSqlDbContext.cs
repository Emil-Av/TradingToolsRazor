using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data
{
    public class PostgreSqlDbContext : ApplicationDbContext
    {
        public PostgreSqlDbContext(DbContextOptions<PostgreSqlDbContext> options) 
            : base(options)
        {
        }
    }
}
