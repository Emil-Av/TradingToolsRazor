using System;
using System.IO;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Factories
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "TradingToolsRazor"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
                .Build();

            var provider = configuration["DatabaseProvider"] ?? "SqlServer";
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? configuration.GetSection("ConnectionStrings")["DefaultConnection"]
                ?? throw new InvalidOperationException("DefaultConnection string is missing.");

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            if (provider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
            {
                builder.UseNpgsql(connectionString, x => x.MigrationsAssembly("DataAccess"));
            }
            else
            {
                builder.UseSqlServer(connectionString, x => x.MigrationsAssembly("DataAccess"));
            }

            return new ApplicationDbContext(builder.Options);
        }
    }
}
