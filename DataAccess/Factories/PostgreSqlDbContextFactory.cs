using System;
using System.IO;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Factories
{
    public class PostgreSqlDbContextFactory : IDesignTimeDbContextFactory<PostgreSqlDbContext>
    {
        public PostgreSqlDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "TradingToolsRazor"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
                .Build();

            var connectionString = configuration.GetConnectionString("PostgreSqlConnection")
                ?? throw new InvalidOperationException("PostgreSqlConnection string is missing.");

            var builder = new DbContextOptionsBuilder<PostgreSqlDbContext>();
            builder.UseNpgsql(connectionString, x => x.MigrationsAssembly("DataAccess"));

            return new PostgreSqlDbContext(builder.Options);
        }
    }
}
