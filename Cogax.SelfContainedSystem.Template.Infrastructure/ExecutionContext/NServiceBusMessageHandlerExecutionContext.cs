using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Infrastructure.UnitOfWork;

using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

public class NServiceBusMessageHandlerExecutionContext : IExecutionContext
{
    private readonly IServiceProvider _serviceProvider;

    public NServiceBusMessageHandlerExecutionContext(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ExecutionContextOutboxType GetExecutionContextOutboxType()
        => ExecutionContextOutboxType.NServiceBusOutbox;

    public IUnitOfWork CreateUnitOfWork() =>
        _serviceProvider.GetRequiredService<NServiceBusUnitOfWork>();
}
