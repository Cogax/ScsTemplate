using System.Transactions;

using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Core.Application.Common.Ports;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.UnitOfWork;

public class TransactionScopeUnitOfWork : IUnitOfWork
{
    private readonly IServiceProvider _serviceProvider;
    private readonly WriteModelDbContext _dbContext;

    public TransactionScopeUnitOfWork(
        IServiceProvider serviceProvider,
        WriteModelDbContext dbContext)
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
                using var transcation = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                var result = await operation(cToken);
                await _dbContext.SaveChangesAsync(cToken);
                transcation.Complete();
                return result;
            },
            cancellationToken: cancellationToken);
    }
}
