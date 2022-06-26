using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.UnitOfWork;

public class NullUnitOfWork : IUnitOfWork
{
    public async Task<T> ExecuteOperation<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
    {
        return await operation(cancellationToken);
    }
}
