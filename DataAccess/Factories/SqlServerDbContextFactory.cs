using System;
using System.IO;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Factories
{
    public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
    {
        public SqlServerDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "TradingToolsRazor"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
                .Build();

            // First try SqlServerConnection, then DefaultConnection as fallback
            var connectionString = configuration.GetConnectionString("SqlServerConnection")
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("SqlServerConnection or DefaultConnection string is missing.");

            var builder = new DbContextOptionsBuilder<SqlServerDbContext>();
            builder.UseSqlServer(connectionString, x => x.MigrationsAssembly("DataAccess"));

            return new SqlServerDbContext(builder.Options);
        }
    }
}
