using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Generated;

using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Contexts
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

        public virtual DbSet<PocNsbOutboxWebWebOutbox> PocNsbOutboxWebWebOutbox { get; set; } = null!;
        public virtual DbSet<PocNsbOutboxWebWebOutboxDelayed> PocNsbOutboxWebWebOutboxDelayed { get; set; } = null!;
        public virtual DbSet<Poison> Poison { get; set; } = null!;
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
            modelBuilder.Entity<PocNsbOutboxWebWebOutbox>(entity =>
            {
                entity.HasIndex(e => e.Expires, "Index_Expires")
                    .HasFilter("([Expires] IS NOT NULL)");

                entity.Property(e => e.RowVersion).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PocNsbOutboxWebWebOutboxDelayed>(entity =>
            {
                entity.Property(e => e.RowVersion).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Poison>(entity =>
            {
                entity.HasIndex(e => e.Expires, "Index_Expires")
                    .HasFilter("([Expires] IS NOT NULL)");

                entity.Property(e => e.RowVersion).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<TodoItems>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
