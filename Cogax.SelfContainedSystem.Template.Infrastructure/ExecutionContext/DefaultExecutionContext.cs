using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.Consistency;
using Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext.Consistency.Outbox;

using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

public class DefaultExecutionContext : IExecutionContext
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultExecutionContext(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPersistenceTransaction CreatePersistenceTransaction() =>
        _serviceProvider.GetRequiredService<TransactionScopePersistenceTransaction>();

    public IOutbox CreateOutbox() =>
        _serviceProvider.GetRequiredService<HangfireOutbox>();
}
