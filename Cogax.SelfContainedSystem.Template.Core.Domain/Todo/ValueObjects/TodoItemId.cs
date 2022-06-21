using System.Diagnostics;

using Ardalis.GuardClauses;

using Cogax.SelfContainedSystem.Template.Core.Domain.Common;

namespace Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

[DebuggerDisplay("{Value}")]
public class TodoItemId : ValueObject
{
    public Guid Value { get; }

    [DebuggerStepThrough]
    public TodoItemId(Guid value)
    {
        Guard.Against.Default(value, nameof(value));
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
