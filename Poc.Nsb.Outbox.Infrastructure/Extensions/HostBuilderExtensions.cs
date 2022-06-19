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
            endpointConfiguration.EnableInstallers(); // Damit Queues etc. beim Startup erstellt werden, falls nicht vorhanden (DEV Mode)
            endpointConfiguration.PurgeOnStartup(true); // Damit Queues beim Startup immer leer sind (DEV Mode)
            endpointConfiguration.EnableOutbox(); // Outbox aktivieren

            // Persistence
            // Die NSB Persistenz definiert wo die persistenten Daten gespeichert werden. Dies sind Daten wie
            // Outbox, Saga Daten, etc.
            var sqlPersistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            sqlPersistence.ConnectionBuilder(() => new SqlConnection(hostBuilderContext.Configuration["ConnectionStrings:Db"]));
            sqlPersistence.SqlDialect<SqlDialect.MsSqlServer>();

            // Transport
            // Der NSB Transport definiert wo die Messages gesendet werden. Es gibt Transporte für
            // RabbitMq, Azure ServiceBus, MSSQL, etc.
            var rabbitMqTransport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            rabbitMqTransport.ConnectionString(hostBuilderContext.Configuration["ConnectionStrings:Bus"]);
            rabbitMqTransport.UseConventionalRoutingTopology();
            
            // SendOnly Endpunkte sind Artefakte, welche nur Messages Publizieren und/oder senden
            // aber keine Abonnieren, Handeln oder Subscriben.
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
