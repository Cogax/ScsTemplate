using Microsoft.EntityFrameworkCore;
using Poc.Nsb.Outbox.Core;

namespace Poc.Nsb.Outbox.Infrastructure;

public class PocDbContext : DbContext
{
    public DbSet<MyEntity> MyEntities { get; set; }

    public PocDbContext(DbContextOptions<PocDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PocDbContext).Assembly);
    }
}