using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Hangfire;

using Hangfire.Server;

using Microsoft.Extensions.DependencyInjection;
using NServiceBus.Persistence.Sql;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.ExecutionContext;

public class ExecutionContextFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ExecutionContextFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IExecutionContext Create()
    {
        var hangfireContext = _serviceProvider.GetService<PerformContext>();
        if (hangfireContext?.Items.TryGetValue(HangfireOutboxUniformSession.JobParameterName, out var value) == true &&
            (string)value == HangfireOutboxUniformSession.JobParameterValue)
        {
            return _serviceProvider.GetRequiredService<HangfireOutboxJobExecutionContext>();
        }

        if (hangfireContext != null)
            return _serviceProvider.GetRequiredService<HangfireJobExecutionContext>();

        try // TODO: Better way to identiy NsbMessageHandlerContext, maybe via similar as IUnifiedSession
        {
            if (_serviceProvider.GetService<ISqlStorageSession>() != null)
                return _serviceProvider.GetRequiredService<NServiceBusMessageHandlerExecutionContext>();
        } catch{}
        

        return _serviceProvider.GetRequiredService<DefaultExecutionContext>();
    }
}
