using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Persistence.Sql;

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
        try // TODO: Better way to identiy NsbMessageHandlerContext, maybe via similar as IUnifiedSession
        {
            if (_serviceProvider.GetService<ISqlStorageSession>() != null)
                return _serviceProvider.GetRequiredService<NServiceBusMessageHandlerExecutionContext>();
        } catch{}
        

        return _serviceProvider.GetRequiredService<DefaultExecutionContext>();
    }
}
