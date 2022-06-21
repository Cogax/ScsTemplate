using System.Data.Common;

using Cogax.SelfContainedSystem.Template.Extensions.NServiceBus.WebOutbox;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.Contexts;
using Cogax.SelfContainedSystem.Template.Infrastructure.HostedServices;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddMessaging(this IHostBuilder hostBuilder,
        string endpointName,
        bool enableSendOnly,
        bool enableWebOutbox,
        bool enableNsbOutbox)
    {
        hostBuilder.UseNServiceBus(hostBuilderContext =>
        {
            EndpointConfiguration endpointConfiguration = new(endpointName);
            endpointConfiguration.EnableInstallers(); // Damit Queues etc. beim Startup erstellt werden, falls nicht vorhanden (DEV Mode)
            endpointConfiguration.PurgeOnStartup(true); // Damit Queues beim Startup immer leer sind (DEV Mode)
            if(enableNsbOutbox) {
                endpointConfiguration.EnableOutbox(); // Outbox aktivieren
            }

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
            Action<TransportExtensions<RabbitMQTransport>> configureRabbitMq = rabbitMqTransport =>
            {
                rabbitMqTransport.ConnectionString(hostBuilderContext.Configuration["ConnectionStrings:Bus"]);
                rabbitMqTransport.UseConventionalRoutingTopology();
            };
            configureRabbitMq(rabbitMqTransport);

            if (enableWebOutbox)
            {
                var webOutboxConfiguration = new WebOutboxConfiguration(
                    outboxEndpointName: $"{endpointName}.WebOutbox",
                    destinationEndpointName: endpointName,
                    poisonMessageQueue: "poison");

                webOutboxConfiguration.ConfigureOutboxTransport<SqlServerTransport>(transport =>
                {
                    transport.ConnectionString(hostBuilderContext.Configuration["ConnectionStrings:Db"]);
                });

                webOutboxConfiguration.ConfigureDestinationTransport<RabbitMQTransport>(configureRabbitMq);
                webOutboxConfiguration.AutoCreateQueues();

                hostBuilder.ConfigureServices((context, services) =>
                {
                    // TODO: Evtl. Fallback, dass eine neue Connection und / oder Transaction aufgebaut wird,
                    // falls keine vorhanden ist.
                    
                    services.AddTransient<DbTransaction>(sp =>
                    {
                        var dbContext = sp.GetRequiredService<WriteModelDbContext>();
                        dbContext.Database.BeginTransaction();
                        var transaction = dbContext.Database.CurrentTransaction;

                        if (transaction == null)
                            throw new Exception("Transaction soll nicht null sein! Die Transaktion sollte in der Unit Of Work eröffnet werden!");

                        return transaction.GetDbTransaction();
                    });
                    
                    services.AddSingleton<WebOutboxConfiguration>(webOutboxConfiguration);
                    services.AddSingleton<WebOutbox>(sp =>
                        sp.GetRequiredService<WebOutboxConfiguration>().WebOutbox ??
                        throw new Exception("WebOutbox is null"));
                    services.AddTransient<WebOutboxMessageSession>(sp =>
                        (WebOutboxMessageSession)sp.GetRequiredService<WebOutbox>().CreateMessageSession(() =>
                            TransportTransactionFactory.CreateFromDbTransaction(
                                sp.GetRequiredService<DbTransaction>())));
                    services.AddHostedService<WebOutboxStarter>();
                });
            }
            
            // SendOnly Endpunkte sind Artefakte, welche nur Messages Publizieren und/oder senden
            // aber keine Abonnieren, Handeln oder Subscriben.
            if (enableSendOnly)
                endpointConfiguration.SendOnly();

            return endpointConfiguration;
        });

        return hostBuilder;
    }
}
