namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Exceptions;

public class AggregateRootNotFoundException : Exception
{
    public IEnumerable<KeyValuePair<string, object>> Arguments { get; }

    public AggregateRootNotFoundException(
        string aggregateName,
        params KeyValuePair<string, object>[] arguments)
        : base($"{aggregateName} not found!")
    {
        Arguments = arguments;
    }
}
