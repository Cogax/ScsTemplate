using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

using NServiceBus;
using NServiceBus.Persistence.Sql;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Messaging.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddMessaging(this IHostBuilder hostBuilder,
        string endpointName,
        bool enableSendOnly,
        bool enablePurgeAtStartup = false)
    {
        hostBuilder.UseNServiceBus(hostBuilderContext =>
        {
            EndpointConfiguration endpointConfiguration = new(endpointName);
            endpointConfiguration.EnableInstallers(); // Damit Queues etc. beim Startup erstellt werden, falls nicht vorhanden (DEV Mode)
            endpointConfiguration.PurgeOnStartup(enablePurgeAtStartup); // Damit Queues beim Startup immer leer sind (DEV Mode)
            endpointConfiguration // Tests
                .Recoverability()
                .Immediate(i => i.NumberOfRetries(0))
                .Delayed(d => d.NumberOfRetries(0));
            endpointConfiguration.EnableOutbox(); // Messaging Context Outbox aktivieren

            // Persistence
            // Die NSB Persistenz definiert wo die persistenten Daten gespeichert werden. Dies sind Daten wie
            // Outbox, Saga Daten, etc.
            var sqlPersistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            sqlPersistence.ConnectionBuilder(() => new SqlConnection(hostBuilderContext.Configuration["ConnectionStrings:Db"]));
            sqlPersistence.SqlDialect<SqlDialect.MsSqlServer>();
            sqlPersistence.TablePrefix("NSB_");

            // Transport
            // Der NSB Transport definiert wo die Messages gesendet werden. Es gibt Transporte für
            // RabbitMq, Azure ServiceBus, MSSQL, etc.
            var rabbitMqTransport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            rabbitMqTransport.ConnectionString(hostBuilderContext.Configuration["ConnectionStrings:Bus"]);
            rabbitMqTransport.UseConventionalRoutingTopology();

            // Unit of Work
            endpointConfiguration.RegisterComponents(c =>
            {
                c.ConfigureComponent(b =>
                {
                    var session = b.Build<ISqlStorageSession>();
                    var context = b.Build<WriteModelDbContext>();
                    context.Database.UseTransaction(session.Transaction);
                    session.OnSaveChanges(s => context.SaveChangesAsync());
                    return context;
                }, DependencyLifecycle.InstancePerUnitOfWork);
            });

            // SendOnly Endpunkte sind Artefakte, welche nur Messages Publizieren und/oder senden
            // aber keine Abonnieren, Handeln oder Subscriben.
            if (enableSendOnly)
                endpointConfiguration.SendOnly();

            return endpointConfiguration;
        });

        return hostBuilder;
    }
}
