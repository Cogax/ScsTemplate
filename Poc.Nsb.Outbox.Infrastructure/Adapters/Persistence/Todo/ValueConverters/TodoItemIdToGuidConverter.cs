using System.Data;

using Dapper;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Poc.Nsb.Outbox.Core.Domain.Todo.ValueObjects;

namespace Poc.Nsb.Outbox.Infrastructure.Adapters.Persistence.Todo.ValueConverters;

// EF Core
public class TodoItemIdToGuidConverter : ValueConverter<TodoItemId, Guid>
{
    public TodoItemIdToGuidConverter() : base(
        v => v.Value,
        v => new TodoItemId(v))
    { }
}

// Dapper
internal sealed class SortieranlageIdToLongTypeHandler : SqlMapper.TypeHandler<TodoItemId>
{
    public override void SetValue(IDbDataParameter parameter, TodoItemId value)
    {
        parameter.Value = value.Value;
    }

    public override TodoItemId Parse(object v)
    {
        return new TodoItemId(Guid.Parse((string)v));
    }
}
