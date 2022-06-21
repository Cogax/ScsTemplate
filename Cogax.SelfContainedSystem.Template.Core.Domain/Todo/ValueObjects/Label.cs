using System.Diagnostics;

using Ardalis.GuardClauses;

using Cogax.SelfContainedSystem.Template.Core.Domain.Common;

namespace Cogax.SelfContainedSystem.Template.Core.Domain.Todo.ValueObjects;

[DebuggerDisplay("{Value}")]
public class Label : ValueObject
{
    public const int MaxLength = 250;
    public string Value { get; }

    [DebuggerStepThrough]
    public Label(string value)
    {
        Guard.Against.NullOrWhiteSpace(value, nameof(value));
        if (value.Length > MaxLength) throw new ArgumentException($"Maxlength of {MaxLength} was reached!", nameof(value));
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
