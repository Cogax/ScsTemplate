using System.Transactions;

using Cogax.SelfContainedSystem.Template.Core.Application.Common.Exceptions;
using Cogax.SelfContainedSystem.Template.Core.Application.Common.Ports;
using Cogax.SelfContainedSystem.Template.Core.Domain.Common;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence;

public class SqlServerPersistenceAdapter : IPersistenceAdapter
{
    private readonly WriteModelDbContext dbContext;
    private readonly IServiceProvider _serviceProvider;

    public SqlServerPersistenceAdapter(WriteModelDbContext dbContext, IServiceProvider serviceProvider)
    {
        this.dbContext = dbContext;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// See https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency#execution-strategies-and-transactions
    /// </summary>
    public async Task<T> ExecuteWithTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken)
    {
        // Create an Execution Strategy via a separate DbContext in order to support "Ambient Transactions".
        var strategy = new WriteModelDbContext(_serviceProvider.GetRequiredService<DbContextOptions<WriteModelDbContext>>())
            .Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(
            operation: async (cToken) =>
            {
                using var transcation = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                var result = await operation(cToken);
                await SaveChangesAsync(cToken);
                transcation.Complete();
                return result;
            },
            cancellationToken: cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException concurrencyException)
        {
            throw new AggregateRootConcurrencyException(
                await GetAllTrackedAggregatesAsync(cancellationToken),
                concurrencyException);
        }
    }

    public Task<IReadOnlyCollection<IAggregateRoot>> GetAllTrackedAggregatesAsync(CancellationToken cancellationToken)
    {
        var aggregates = dbContext.ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(x => x.Entity)
            .ToList();

        return Task.FromResult<IReadOnlyCollection<IAggregateRoot>>(aggregates);
    }
}
