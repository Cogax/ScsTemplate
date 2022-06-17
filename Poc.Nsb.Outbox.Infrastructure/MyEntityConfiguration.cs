using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poc.Nsb.Outbox.Core;

namespace Poc.Nsb.Outbox.Infrastructure;

public class MyEntityConfiguration : IEntityTypeConfiguration<MyEntity>
{
    public void Configure(EntityTypeBuilder<MyEntity> builder)
    {
        builder.HasKey(x => x.Id);
    }
}