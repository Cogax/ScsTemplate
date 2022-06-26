using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.UnitOfWork;

// Im Fall von NSB wird bei der WriteModelDbContext registrierung
// der DbContext bereits mit der NSB Storage Session aufgesetzt.
public class NServiceBusUnitOfWork : IUnitOfWork
{
    private readonly IServiceProvider _serviceProvider;
    private readonly WriteModelDbContext _dbContext;

    public NServiceBusUnitOfWork(IServiceProvider serviceProvider, WriteModelDbContext dbContext)
    {
        _serviceProvider = serviceProvider;
        _dbContext = dbContext;
    }

    public async Task<T> ExecuteOperation<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
    {
        // Hangfire only supports AmbientTransactions (via TransactionScope)
        // Create an Execution Strategy via a separate DbContext in order to support "Ambient Transactions".
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
