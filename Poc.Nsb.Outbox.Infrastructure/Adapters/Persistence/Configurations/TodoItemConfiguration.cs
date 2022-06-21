using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Poc.Nsb.Outbox.Core.Domain.Todo.Aggregates;
using Poc.Nsb.Outbox.Core.Domain.Todo.ValueObjects;

namespace Poc.Nsb.Outbox.Infrastructure.Adapters.Persistence.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("TodoItems").HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("Id");
        builder.Property<Label>("label").HasColumnName("Label").HasMaxLength(Label.MaxLength);
        builder.Property<bool>("completed").HasColumnName("Completed");
    }
}
