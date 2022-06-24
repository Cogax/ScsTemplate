using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Generated;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts
{
    public partial class ReadModelDbContext : DbContext
    {
        public ReadModelDbContext()
        {
        }

        public ReadModelDbContext(DbContextOptions<ReadModelDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AggregatedCounter> AggregatedCounter { get; set; } = null!;
        public virtual DbSet<Counter> Counter { get; set; } = null!;
        public virtual DbSet<Hash> Hash { get; set; } = null!;
        public virtual DbSet<Job> Job { get; set; } = null!;
        public virtual DbSet<JobParameter> JobParameter { get; set; } = null!;
        public virtual DbSet<JobQueue> JobQueue { get; set; } = null!;
        public virtual DbSet<List> List { get; set; } = null!;
        public virtual DbSet<NsbOutboxData> NsbOutboxData { get; set; } = null!;
        public virtual DbSet<Schema> Schema { get; set; } = null!;
        public virtual DbSet<Server> Server { get; set; } = null!;
        public virtual DbSet<Set> Set { get; set; } = null!;
        public virtual DbSet<State> State { get; set; } = null!;
        public virtual DbSet<TodoItems> TodoItems { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=localhost;database=PocDb;user=sa;password=Top-Secret;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AggregatedCounter>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_AggregatedCounter_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.HasIndex(e => e.Key, "CX_HangFire_Counter")
                    .IsClustered();
            });

            modelBuilder.Entity<Hash>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Hash_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Job_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => e.StateName, "IX_HangFire_Job_StateName")
                    .HasFilter("([StateName] IS NOT NULL)");
            });

            modelBuilder.Entity<JobParameter>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameter)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueue>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_List_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<NsbOutboxData>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK__NSB_Outb__C87C0C9DEF92A7A1")
                    .IsClustered(false);

                entity.HasIndex(e => e.DispatchedAt, "Index_DispatchedAt")
                    .HasFilter("([Dispatched]=(1))");
            });

            modelBuilder.Entity<Schema>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Set_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.State)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            modelBuilder.Entity<TodoItems>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Removed).HasDefaultValueSql("(CONVERT([bit],(0)))");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
