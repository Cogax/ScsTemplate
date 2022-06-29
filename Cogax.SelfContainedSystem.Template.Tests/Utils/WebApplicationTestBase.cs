using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.SignalR;
using Cogax.SelfContainedSystem.Template.Web;
using Cogax.SelfContainedSystem.Template.Worker;
using EasyNetQ.Management.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Text.Json;

using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;
using EasyNetQ.Management.Client.Model;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

namespace Cogax.SelfContainedSystem.Template.Tests.Utils;

public abstract class WebApplicationTestBase
{
    protected const string WebQueue = "Cogax.SelfContainedSystem.Template.Web";
    protected const string WorkerQueue = "Cogax.SelfContainedSystem.Template.Worker";
    protected const string ErrorQueue = "error";
    protected static TimeSpan WaitDuration = TimeSpan.FromSeconds(5);
    protected TestableWebApplication<WebProgram> Web = null!;
    protected TestableWebApplication<WorkerProgram> Worker = null!;
    protected HttpClient WebClient = null!;
    protected HttpClient WorkerClient = null!;
    protected IManagementClient RabbitMqClient = null!;

    protected Mock<ISignalRPublisher> SignalRPublisherMock = null!;
    protected Mock<IChaosMonkey> ChaosMonkeyMock = null!;

    [TestInitialize]
    public async Task SetUp()
    {
        SignalRPublisherMock = new Mock<ISignalRPublisher>();
        ChaosMonkeyMock = new Mock<IChaosMonkey>();

        Action<IServiceCollection> servicesOverride = (services) =>
        {
            services.AddSingleton(SignalRPublisherMock.Object);
            services.AddSingleton(ChaosMonkeyMock.Object);
        };

        Web = new TestableWebApplication<WebProgram>();
        Worker = new TestableWebApplication<WorkerProgram>();

        Web.ConfigureServices(servicesOverride);
        Worker.ConfigureServices(servicesOverride);
        
        RabbitMqClient = new ManagementClient("http://localhost", "guest", "guest", portNumber: 15672);

        await PurgeDb();
        await PurgeRabbitMq();

        WebClient = Web.CreateClient();
        WorkerClient = Worker.CreateClient();
    }

    [TestCleanup]
    public async Task TearDown()
    {
        RabbitMqClient.Dispose();

        WebClient.Dispose();
        Web.Server.Dispose();
        await Web.DisposeAsync();

        WorkerClient.Dispose();
        Worker.Server.Dispose();
        await Worker.DisposeAsync();
    }

    private async Task PurgeRabbitMq()
    {
        await PurgeRabbitMqQueue(WorkerQueue);
        await PurgeRabbitMqQueue(WebQueue);
        await PurgeRabbitMqQueue(ErrorQueue);
    }

    private async Task PurgeRabbitMqQueue(string queueName)
    {
        var vhost = await RabbitMqClient.GetVhostAsync("/");
        var queue = await RabbitMqClient.GetQueueAsync(queueName, vhost);
        await RabbitMqClient.PurgeAsync(queue);
    }

    protected async Task PurgeDb()
    {
        using var scope = Web.Services.CreateScope();
        var db = scope.ServiceProvider.GetService<ReadModelDbContext>();

        await db.Database.ExecuteSqlRawAsync(@"
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NSB_OutboxData')
BEGIN
TRUNCATE TABLE [dbo].[NSB_OutboxData]
END
TRUNCATE TABLE [dbo].[TodoItems]
TRUNCATE TABLE [HangFire].[AggregatedCounter]
TRUNCATE TABLE [HangFire].[Counter]
TRUNCATE TABLE [HangFire].[JobParameter]
TRUNCATE TABLE [HangFire].[JobQueue]
TRUNCATE TABLE [HangFire].[List]
TRUNCATE TABLE [HangFire].[State]
DELETE FROM [HangFire].[Job]
DBCC CHECKIDENT ('[HangFire].[Job]', reseed, 0)
UPDATE [HangFire].[Hash] SET Value = 1 WHERE Field = 'LastJobId'");
        
        scope.Dispose();
    }

    protected async Task AssertRabbitMqQueueLength(string queueName, int count)
    {
        var vhost = await RabbitMqClient.GetVhostAsync("/");
        var queue = await RabbitMqClient.GetQueueAsync(queueName, vhost);
        var messages = await RabbitMqClient.GetMessagesFromQueueAsync(queue, new GetMessagesCriteria(100, Ackmodes.ack_requeue_true));

        messages.Should().HaveCount(count, "Found messages in queue '{0}': {1}", queueName, JsonSerializer.Serialize(messages));
    }

    protected async Task AssertHangfireJobs(int total, int succeeded)
    {
        using var scope = Web.Services.CreateScope();
        var jobs = await scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().Job.ToListAsync();
        jobs.Should().HaveCount(total);
        jobs.Where(j => j.StateName == "Succeeded").Should().HaveCount(succeeded);
        scope.Dispose();
    }
}
