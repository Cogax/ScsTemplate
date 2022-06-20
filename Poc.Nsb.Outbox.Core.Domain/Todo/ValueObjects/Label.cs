using System.Diagnostics;

using Ardalis.GuardClauses;

using Poc.Nsb.Outbox.Core.Domain.Common;

namespace Poc.Nsb.Outbox.Core.Domain.Todo.ValueObjects;

[DebuggerDisplay("{Value}")]
public class Label : ValueObject
{
    public string Value { get; }

    [DebuggerStepThrough]
    public Label(string value)
    {
        Guard.Against.NullOrWhiteSpace(value, nameof(value));
        if (value.Length >= 250) throw new ArgumentException("Maxlength of 250 was reached!", nameof(value));
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
