using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.NsbExectionContextIdentifier;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;

using NServiceBus;

namespace Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.NServiceBus.Extensions;

public static class NServiceBusHostBuilderExtensions
{
    public static IHostBuilder AddMessaging(this IHostBuilder hostBuilder, string endpointName)
    {
        hostBuilder.UseNServiceBus(hostBuilderContext =>
        {
            EndpointConfiguration endpointConfiguration = new(endpointName);
            endpointConfiguration.EnableInstallers(); // Damit Queues etc. beim Startup erstellt werden, falls nicht vorhanden (DEV Mode)
            endpointConfiguration.PurgeOnStartup(true); // Damit Queues beim Startup immer leer sind (DEV Mode)
            endpointConfiguration // Tests
                .Recoverability()
                .Immediate(i => i.NumberOfRetries(0))
                .Delayed(d => d.NumberOfRetries(0));

            var outbox = endpointConfiguration.EnableOutbox(); // Messaging Context Outbox aktivieren
            outbox.DisableCleanup(); // Only tests
            outbox.UseTransactionScope();

            endpointConfiguration.EnableUniformSession();
            endpointConfiguration.EnableFeature<NsbExecutionContextIdentifierFeature>();

            // By default NServiceBus endpoints scan and load all assemblies found in the bin directory.
            // This means that if more than one endpoint is loaded into the same process all endpoints will
            // scan the same bin directory and all types related to NServiceBus, such as message handlers
            // and/or sagas, are loaded by all endpoints. This can issues to endpoints running in end-to-end
            // tests. It's suggested to configure the endpoint configuration to scan only a limited set of
            // assemblies, and exclude those not related to the current endpoint.
            string otherEntdpoint = endpointName.EndsWith("Worker")
                ? endpointName.Replace("Worker", "Web")
                : endpointName.Replace("Web", "Worker");

            endpointConfiguration.AssemblyScanner().ExcludeAssemblies(
                $"{otherEntdpoint}.dll", "Cogax.SelfContainedSystem.Template.Tests.dll");

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

            return endpointConfiguration;
        });

        return hostBuilder;
    }
}
