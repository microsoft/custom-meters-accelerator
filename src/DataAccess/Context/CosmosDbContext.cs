﻿using ManagedApplicationScheduler.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManagedApplicationScheduler.DataAccess.Context
{
    public class CosmosDbContext : DbContext
    {
 
        public DbSet<Subscription> Subscription { get; set; } = null!;
        public DbSet<UsageResult> UsageResult { get; set; } = null!;
        public DbSet<ScheduledTasks> ScheduledTasks { get; set; } = null!;
        public DbSet<ApplicationLog> ApplicationLog { get; set; } = null!;
        public DbSet<Plan> Plan { get; set; } = null!;
        public DbSet<Payment> Payment { get; set; } = null!;

        public DbSet<ApplicationConfiguration> ApplicationConfiguration { get; set; } = null!;

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
            modelBuilder.Entity<ApplicationLog>()
                .ToContainer("ApplicationLog");
            modelBuilder.Entity<ApplicationConfiguration>()
                .ToContainer("ApplicationConfiguration");
            modelBuilder.Entity<Payment>()
                .ToContainer("Payment");
            modelBuilder.Entity<Plan>()
                            .ToContainer("Plan");
            #endregion

            #region NoDiscriminator
            modelBuilder.Entity<Subscription>().HasNoDiscriminator();
            modelBuilder.Entity<ScheduledTasks>().HasNoDiscriminator();
            modelBuilder.Entity<UsageResult>().HasNoDiscriminator();
            modelBuilder.Entity<ApplicationLog>().HasNoDiscriminator();
            modelBuilder.Entity<ApplicationConfiguration>().HasNoDiscriminator();
            modelBuilder.Entity<Payment>().HasNoDiscriminator();
            modelBuilder.Entity<Plan>().HasNoDiscriminator();
            #endregion

            #region PartitionKey
            modelBuilder.Entity<Subscription>().HasPartitionKey(o => o.PartitionKey);
            modelBuilder.Entity<UsageResult>().HasPartitionKey(o => o.PartitionKey);
            modelBuilder.Entity<ScheduledTasks>().HasPartitionKey(o => o.PartitionKey);
            modelBuilder.Entity<ApplicationLog>().HasPartitionKey(o => o.PartitionKey);
            modelBuilder.Entity<ApplicationConfiguration>().HasPartitionKey(o => o.PartitionKey);
            modelBuilder.Entity<Payment>().HasPartitionKey(o => o.PartitionKey);
            modelBuilder.Entity<Plan>().HasPartitionKey(o => o.PartitionKey);

            #endregion

            #region ETag
            modelBuilder.Entity<Subscription>().UseETagConcurrency();
            modelBuilder.Entity<UsageResult>().UseETagConcurrency();
            modelBuilder.Entity<ScheduledTasks>().UseETagConcurrency();
            modelBuilder.Entity<ApplicationLog>().UseETagConcurrency();
            modelBuilder.Entity<ApplicationConfiguration>().UseETagConcurrency();
            modelBuilder.Entity<Payment>().UseETagConcurrency();
            modelBuilder.Entity<Plan>().UseETagConcurrency();

            #endregion



        }
    }
}
