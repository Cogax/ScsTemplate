using Cogax.SelfContainedSystem.Template.Extensions.NServiceBus.WebOutbox;

using Microsoft.Extensions.Hosting;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Messaging.HostedServices;

public class WebOutboxStarter : IHostedService
{
    private readonly WebOutboxConfiguration _webOutboxConfiguration;

    public WebOutboxStarter(WebOutboxConfiguration webOutboxConfiguration)
    {
        _webOutboxConfiguration = webOutboxConfiguration;
    }

    public async Task StartAsync(CancellationToken cancellationToken) => await _webOutboxConfiguration.StartOutbox();

    public async Task StopAsync(CancellationToken cancellationToken) => await _webOutboxConfiguration.StopOutbox();
}
