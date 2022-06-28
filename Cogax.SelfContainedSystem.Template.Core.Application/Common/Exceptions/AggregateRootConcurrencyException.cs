namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Exceptions;

public class AggregateRootConcurrencyException : Exception
{
    public AggregateRootConcurrencyException(Exception innerException)
        : base($"Concurrency Exception occured", innerException)
    {
    }
}
