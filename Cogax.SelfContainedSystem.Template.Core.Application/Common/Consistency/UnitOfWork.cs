namespace Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

public interface IUnitOfWork
{
    Task<T> ExecuteOperation<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken);
}
