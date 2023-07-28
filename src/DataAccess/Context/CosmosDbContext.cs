using ManagedApplicationScheduler.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagedApplicationScheduler.DataAccess.Context
{
    public class CosmosDbContext : DbContext
    {
        private DbContextOptionsBuilder<CosmosDbContext> dbContextOptionsBuilder;

        public DbSet<Subscription> Subscription { get; set; }
        public DbSet<UsageResult> UsageResult { get; set; }
        public DbSet<ScheduledTasks> ScheduledTasks { get; set; }


        public CosmosDbContext(DbContextOptions<CosmosDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region DefaultContainer
            modelBuilder.HasDefaultContainer("Subscription");
            #endregion

            #region Container
            modelBuilder.Entity<Subscription>()
                .ToContainer("Subscription");
            modelBuilder.Entity<UsageResult>()
                .ToContainer("UsageResult");
            modelBuilder.Entity<ScheduledTasks>()
                .ToContainer("ScheduledTasks");

            #endregion

            #region NoDiscriminator
            modelBuilder.Entity<Subscription>().HasNoDiscriminator();
            modelBuilder.Entity<ScheduledTasks>().HasNoDiscriminator();
            modelBuilder.Entity<UsageResult>().HasNoDiscriminator();
            #endregion

            #region PartitionKey
            modelBuilder.Entity<Subscription>().HasPartitionKey(o => o.PartitionKey);
            modelBuilder.Entity<UsageResult>().HasPartitionKey(o => o.PartitionKey);
            modelBuilder.Entity<ScheduledTasks>().HasPartitionKey(o => o.PartitionKey);
            #endregion

            #region ETag
            modelBuilder.Entity<Subscription>().UseETagConcurrency();
            modelBuilder.Entity<UsageResult>().UseETagConcurrency();
            modelBuilder.Entity<ScheduledTasks>().UseETagConcurrency();
            #endregion



        }
    }
}
