using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.NsbExectionContextIdentifier;

using Microsoft.Extensions.DependencyInjection;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

public class ExecutionContextFactory : IExecutionContextFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ExecutionContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IExecutionContext Create()
    {
        var nsbMessageHandlerExecutionContextIdentifier = _serviceProvider.GetRequiredService<INsbMessageHandlerExecutionContextIdentifier>();
        if (nsbMessageHandlerExecutionContextIdentifier is InMessageHandlerExecutionContextIdentifier)
            return _serviceProvider.GetRequiredService<NServiceBusMessageHandlerExecutionContext>();

        return _serviceProvider.GetRequiredService<DefaultExecutionContext>();
    }
}
