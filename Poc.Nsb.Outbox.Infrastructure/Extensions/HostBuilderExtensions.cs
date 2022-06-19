using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;

using NServiceBus;
using NServiceBus.Features;

namespace Poc.Nsb.Outbox.Infrastructure.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddMessaging(this IHostBuilder hostBuilder, string endpointName, bool sendOnly)
    {
        hostBuilder.UseNServiceBus(hostBuilderContext =>
        {
            EndpointConfiguration endpointConfiguration = new(endpointName);

            // Persistence
            var sqlPersistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            sqlPersistence.ConnectionBuilder(() => new SqlConnection(hostBuilderContext.Configuration["ConnectionStrings:Db"]));
            sqlPersistence.SqlDialect<SqlDialect.MsSqlServer>();

            // Transport
            var rabbitMqTransport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            rabbitMqTransport.ConnectionString(hostBuilderContext.Configuration["ConnectionStrings:Bus"]);
            rabbitMqTransport.UseConventionalRoutingTopology();

            endpointConfiguration.PurgeOnStartup(true);
            endpointConfiguration.EnableInstallers();

            if (sendOnly)
            {
                endpointConfiguration.DisableFeature<AutoSubscribe>();
                endpointConfiguration.SendOnly();
            }

            return endpointConfiguration;
        });

        return hostBuilder;
    }
}
