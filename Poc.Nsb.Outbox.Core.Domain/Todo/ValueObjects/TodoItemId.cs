using System.Diagnostics;

using Ardalis.GuardClauses;

using Poc.Nsb.Outbox.Core.Domain.Common;

namespace Poc.Nsb.Outbox.Core.Domain.Todo.ValueObjects;

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
