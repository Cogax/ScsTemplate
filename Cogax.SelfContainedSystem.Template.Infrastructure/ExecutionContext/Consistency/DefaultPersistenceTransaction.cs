using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.Consistency;

public class DefaultPersistenceTransaction : BasePersistenceTransaction
{
    private readonly IServiceProvider _serviceProvider;
    private readonly WriteModelDbContext _dbContext;

    public DefaultPersistenceTransaction(IServiceProvider serviceProvider, WriteModelDbContext dbContext)
    {
        _serviceProvider = serviceProvider;
        _dbContext = dbContext;
    }

    protected override async Task<T> Execute<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
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
