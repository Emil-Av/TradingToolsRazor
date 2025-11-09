using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;

namespace DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        // When the class gets injected, the connection string is passed to the DbContext as a paramater in the constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            base.OnModelCreating(modelBuilder);

            // TPT Configuration: Maps each derived class to its own table
            modelBuilder.Entity<BaseTrade>().ToTable("BaseTrades");
            modelBuilder.Entity<Trade>().ToTable("Trades");
            modelBuilder.Entity<ResearchFirstBarPullback>().ToTable("ResearchFirstBarPullbacks");
            modelBuilder.Entity<ResearchCradle>().ToTable("ResearchCradles");
            modelBuilder.Entity<ResearchCandleBracketing>().ToTable("ResearchCandleBracketing");

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

            // Include SampleSize relationship on BaseTrade
            modelBuilder.Entity<BaseTrade>()
                .HasOne(b => b.SampleSize)
                .WithMany()
                .HasForeignKey(b => b.SampleSizeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
