using System;

using Cogax.SelfContainedSystem.Template.Web;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cogax.SelfContainedSystem.Template.Tests.Utils;

public class TestableWebApplication<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    private Action<IServiceCollection>? _servicesOverride = null;
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            _servicesOverride?.Invoke(services);
        });

        builder.ConfigureLogging(options => options.ClearProviders().AddSimpleConsole());

        return base.CreateHost(builder);
    }

    public void ConfigureServices(Action<IServiceCollection> servicesOverride)
    {
        _servicesOverride = servicesOverride;
    }
}
