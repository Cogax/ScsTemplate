using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.UnitOfWork;

public class DefaultUnitOfWork : IUnitOfWork
{
    private readonly WriteModelDbContext _dbContext;

    public DefaultUnitOfWork(WriteModelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T> ExecuteOperation<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(
            operation: async (cToken) =>
            {
                var result = await operation(cToken);
                await _dbContext.SaveChangesAsync(cToken);
                return result;
            },
            cancellationToken: cancellationToken);
    }
}
