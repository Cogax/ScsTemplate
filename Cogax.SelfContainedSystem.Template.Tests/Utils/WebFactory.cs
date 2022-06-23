using System;

using Cogax.SelfContainedSystem.Template.Web;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cogax.SelfContainedSystem.Template.Tests.Utils;

public class WebFactory : WebApplicationFactory<WebProgram>
{
    private readonly Action<IServiceCollection>? _servicesOverride;

    public WebFactory(Action<IServiceCollection>? servicesOverride = null)
    {
        _servicesOverride = servicesOverride;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            _servicesOverride?.Invoke(services);
        });

        return base.CreateHost(builder);
    }
}
