using Microsoft.Extensions.Hosting;

using Planzer.Pak.Messaging.NServiceBus.WebOutbox;

namespace Poc.Nsb.Outbox.Infrastructure.HostedServices;

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
