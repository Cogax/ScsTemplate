using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.UnitOfWork;

public class DefaultUnitOfWork : IUnitOfWork
{
    private readonly IServiceProvider _serviceProvider;
    private readonly WriteModelDbContext _dbContext;

    public DefaultUnitOfWork(IServiceProvider serviceProvider, WriteModelDbContext dbContext)
    {
        _serviceProvider = serviceProvider;
        _dbContext = dbContext;
    }

    public async Task<T> ExecuteOperation<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
    {
        var strategy = new WriteModelDbContext(_serviceProvider.GetRequiredService<DbContextOptions<WriteModelDbContext>>())
            .Database.CreateExecutionStrategy();

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
