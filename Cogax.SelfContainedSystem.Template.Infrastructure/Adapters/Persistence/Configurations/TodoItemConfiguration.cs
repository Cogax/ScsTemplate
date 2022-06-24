using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Aggregates;
using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("TodoItems").HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("Id");
        builder.Property<Label>("label").HasColumnName("Label").HasMaxLength(Label.MaxLength);
        builder.Property<bool>("completed").HasColumnName("Completed");
        builder.Property<bool>("removed").HasColumnName("Removed");
        builder.Property<int>("version").HasColumnName("Version").IsConcurrencyToken();
        builder.HasIndex("label").IsUnique(); // UoW testing purposes
        builder.HasIndex("removed").IsUnique(); // UoW testing purposes
    }
}
