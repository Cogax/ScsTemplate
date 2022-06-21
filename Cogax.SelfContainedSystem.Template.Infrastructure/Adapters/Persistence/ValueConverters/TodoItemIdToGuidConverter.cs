using System.Data;

using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

using Dapper;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.ValueConverters;

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
