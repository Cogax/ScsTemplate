using System.Transactions;

using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Core.Application.Common.Exceptions;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Consistency;

public class TransactionScopePersistenceTransaction : IPersistenceTransaction
{
    private readonly IServiceProvider _serviceProvider;
    private readonly WriteModelDbContext _dbContext;

    public TransactionScopePersistenceTransaction(
        IServiceProvider serviceProvider,
        WriteModelDbContext dbContext)
    {
        _serviceProvider = serviceProvider;
        _dbContext = dbContext;
    }

    public async Task<T> ExecuteTransaction<T>(Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken)
    {
        try
        {
            // Create a new, explicit strategy to allow ambient transactions.
            // see: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency#execution-strategies-and-transactions
            var strategy = new WriteModelDbContext(_serviceProvider.GetRequiredService<DbContextOptions<WriteModelDbContext>>())
                .Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(
                operation: async (cToken) =>
                {
                    using var transcation = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                    var result = await operation(cToken);
                    await _dbContext.SaveChangesAsync(cToken);
                    transcation.Complete();
                    return result;
                },
                cancellationToken: cancellationToken);
        }
        catch (DbUpdateConcurrencyException concurrencyException)
        {
            throw new AggregateRootConcurrencyException(concurrencyException);
        }
    }
}
