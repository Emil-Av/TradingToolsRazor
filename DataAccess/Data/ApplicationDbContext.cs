using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Models;
using Models.Trades;
using Models.ViewModels;
using System.Linq;
using System.Text.Json;

namespace DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // When the class gets injected, the connection string is passed to the DbContext as a paramater in the constructor
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<ResearchCandleBracketing> ResearchCandleBracketing { get; set; }
        public DbSet<ResearchCradle> ResearchCradles { get; set; }
        public DbSet<BaseTrade> BaseTrades { get; set; }

        public DbSet<Trade> Trades { get; set; }

        public DbSet<ResearchFirstBarPullback> ResearchFirstBarPullbacks { get; set; }
        public DbSet<Journal> Journals { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<SampleSize> SampleSizes { get; set; }

        public DbSet<UserSettings> UserSettings { get; set; }

        public DbSet<SRS> SRS { get; set; }

        public DbSet<BrunchBreak> BrunchBreak { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Provider-specific configuration for ScreenshotsUrls
            ConfigureScreenshots(modelBuilder);

            // TPT Configuration: Maps each derived class to its own table
            modelBuilder.Entity<BaseTrade>().ToTable("BaseTrades");
            modelBuilder.Entity<Trade>().ToTable("Trades");
            modelBuilder.Entity<ResearchFirstBarPullback>().ToTable("ResearchFirstBarPullbacks");
            modelBuilder.Entity<ResearchCradle>().ToTable("ResearchCradles");
            modelBuilder.Entity<ResearchCandleBracketing>().ToTable("ResearchCandleBracketing");
            modelBuilder.Entity<SRS>().ToTable("SRS");
            modelBuilder.Entity<BrunchBreak>().ToTable("BrunchBreak");

            // Configure the primary key inheritance (TPT) - derived -> BaseTrade (shared PK)
            modelBuilder.Entity<Trade>()
                .HasOne<BaseTrade>()
                .WithOne()
                .HasForeignKey<Trade>(t => t.Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ResearchCradle>()
                .HasOne<BaseTrade>()
                .WithOne()
                .HasForeignKey<ResearchCradle>(t => t.Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ResearchFirstBarPullback>()
                .HasOne<BaseTrade>()
                .WithOne()
                .HasForeignKey<ResearchFirstBarPullback>(t => t.Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ResearchCandleBracketing>()
                .HasOne<BaseTrade>()
                .WithOne()
                .HasForeignKey<ResearchCandleBracketing>(t => t.Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SRS>()
                .HasOne<BaseTrade>()
                .WithOne()
                .HasForeignKey<SRS>(t => t.Id)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BrunchBreak>()
                .HasOne<BaseTrade>()
                .WithOne()
                .HasForeignKey<BrunchBreak>(t => t.Id)
                .OnDelete(DeleteBehavior.NoAction);


            // Include SampleSize relationship on BaseTrade
            modelBuilder.Entity<BaseTrade>()
                .HasOne(b => b.SampleSize)
                .WithMany()
                .HasForeignKey(b => b.SampleSizeId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        private void ConfigureScreenshots(ModelBuilder modelBuilder)
        {
            var providerName = Database.ProviderName;

            if (providerName?.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) == true)
            {
                var propertyBuilder = modelBuilder.Entity<BaseTrade>()
                    .Property(b => b.ScreenshotsUrls)
                    .HasColumnType("jsonb")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v ?? new List<string>(), (JsonSerializerOptions)null),
                        v => string.IsNullOrWhiteSpace(v)
                            ? new List<string>()
                            : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>());

                propertyBuilder.Metadata.SetValueComparer(
                    new ValueComparer<List<string>>(
                        (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                        c => c == null
                            ? 0
                            : c.Aggregate(0, (acc, value) => HashCode.Combine(acc, value == null ? 0 : value.GetHashCode())),
                        c => c == null ? null : c.ToList()));
            }
            else
            {
                modelBuilder.Entity<BaseTrade>()
                    .PrimitiveCollection(b => b.ScreenshotsUrls);
            }
        }
    }
}
