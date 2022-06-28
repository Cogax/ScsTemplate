namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

public interface IPersistenceTransaction
{
    Task<T> ExecuteTransaction<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken);
}
