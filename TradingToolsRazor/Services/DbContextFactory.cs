using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TradingToolsRazor.Services
{
    public interface IDbContextFactory
    {
        DbContext CreateDbContext();
    }

    public class DbContextFactory(IConfiguration configuration) : IDbContextFactory
    {
        private readonly IConfiguration _configuration = configuration;

        public DbContext CreateDbContext()
        {
            var dbProvider = _configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";
            
            if (dbProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
            {
                return CreatePostgreSqlContext();
            }
            else
            {
                return CreateSqlServerContext();
            }
        }

        private PostgreSqlDbContext CreatePostgreSqlContext()
        {
            var connectionString = _configuration.GetConnectionString("PostgreSqlConnection")
                ?? _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("PostgreSQL connection string is missing.");

            var options = new DbContextOptionsBuilder<PostgreSqlDbContext>()
                .UseNpgsql(connectionString, x => x.MigrationsAssembly("DataAccess"))
                .Options;

            return new PostgreSqlDbContext(options);
        }

        private SqlServerDbContext CreateSqlServerContext()
        {
            var connectionString = _configuration.GetConnectionString("SqlServerConnection")
                ?? _configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("SQL Server connection string is missing.");

            var options = new DbContextOptionsBuilder<SqlServerDbContext>()
                .UseSqlServer(connectionString, x => x.MigrationsAssembly("DataAccess"))
                .Options;

            return new SqlServerDbContext(options);
        }
    }
}
