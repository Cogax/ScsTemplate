using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Commands;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Readmodels;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.SignalR;
using Cogax.SelfContainedSystem.Template.Tests.Utils;
using Cogax.SelfContainedSystem.Template.Web;
using Cogax.SelfContainedSystem.Template.Worker;

using Dapper;

using EasyNetQ.Management.Client;
using EasyNetQ.Management.Client.Model;

using FluentAssertions;

using Hangfire;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Cogax.SelfContainedSystem.Template.Tests
{
    [TestClass]
    public class OutboxConsistencyTests
    {
        private const string WebQueue = "Cogax.SelfContainedSystem.Template.Web";
        private const string WorkerQueue = "Cogax.SelfContainedSystem.Template.Worker";
        private const string ErrorQueue = "error";
        private static TimeSpan WaitDuration = TimeSpan.FromSeconds(5);
        private TestableWebApplication<WebProgram> _web = null!;
        private TestableWebApplication<WorkerProgram> _worker = null!;
        private HttpClient _webClient = null!;
        private HttpClient _workerClient = null!;
        private IManagementClient _rabbitMqClient = null!;

        private Mock<ISignalRPublisher> _signalRPublisherMock = null!;
        private Mock<IChaosMonkey> _chaosMonkeyMock = null!;

        [TestInitialize]
        public async Task SetUp()
        {
            _signalRPublisherMock = new Mock<ISignalRPublisher>();
            _chaosMonkeyMock = new Mock<IChaosMonkey>();

            Action<IServiceCollection> servicesOverride = (services) =>
            {
                services.AddSingleton(_signalRPublisherMock.Object);
                services.AddSingleton(_chaosMonkeyMock.Object);
            };

            _web = new TestableWebApplication<WebProgram>();
            _worker = new TestableWebApplication<WorkerProgram>();

            _web.ConfigureServices(servicesOverride);
            _worker.ConfigureServices(servicesOverride);
            
            _rabbitMqClient = new ManagementClient("http://localhost", "guest", "guest", portNumber: 15672);

            await PurgeDb();
            await PurgeRabbitMq();

            _webClient = _web.CreateClient();
            _workerClient = _worker.CreateClient();
        }

        [TestCleanup]
        public async Task TearDown()
        {
            _rabbitMqClient.Dispose();

            _webClient.Dispose();
            _web.Server.Dispose();
            await _web.DisposeAsync();

            _workerClient.Dispose();
            _worker.Server.Dispose();
            await _worker.DisposeAsync();
        }

        [TestMethod]
        public async Task CreateTodoItem_WhenNoException_ThenSignalRInvoked()
        {
            // Arrange
            // Act
            await _webClient.PostAsync("/TodoItem?label=test", null);
            await Task.Delay(WaitDuration);

            // Assert
            _signalRPublisherMock.Verify(x => x.NewTodoItem(It.Is<TodoItemDescription>(d => d.Label == "test")), Times.Once);

            await AssertHangfireJobs(total: 1, succeeded: 1);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }

        [TestMethod]
        public async Task CreateTodoItem_WhenExceptionBeforeCommitOccurs_ThenNoSignalRInvoked()
        {
            // Arrange
            _chaosMonkeyMock.Setup(x => x.OnUowCommit()).Throws<Exception>();

            // Act
            var response = await _webClient.PostAsync("/TodoItem?label=test", null);
            response.IsSuccessStatusCode.Should().BeFalse();
            await Task.Delay(WaitDuration);

            // Assert
            _signalRPublisherMock.Verify(x => x.NewTodoItem(It.Is<TodoItemDescription?>(d => d == null)), Times.Never);

            await AssertHangfireJobs(total: 0, succeeded: 0);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }

        [TestMethod]
        public async Task CreateTodoItem_WhenExceptionBecauseOfLabelUniqueConstraint_ThenNoSignalRInvoked()
        {
            // Arrange
            var response1 = await _webClient.PostAsync("/TodoItem?label=test", null);
            response1.IsSuccessStatusCode.Should().BeTrue();

            // Act
            var response = await _webClient.PostAsync("/TodoItem?label=test", null);
            response.IsSuccessStatusCode.Should().BeFalse();
            await Task.Delay(WaitDuration);

            // Assert
            _signalRPublisherMock.Verify(x => x.NewTodoItem(It.IsAny<TodoItemDescription>()), Times.Once);

            await AssertHangfireJobs(total: 1, succeeded: 1);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }
        
        [TestMethod]
        public async Task CompleteTodoItem_WhenNoException_ThenSignalRInvoked()
        {
            // Arrange
            var response1 = await _webClient.PostAsync("/TodoItem?label=test", null);
            response1.IsSuccessStatusCode.Should().BeTrue();

            var response2 = await _webClient.GetAsync("/TodoItem");
            response2.IsSuccessStatusCode.Should().BeTrue();
            var todoItem = JsonSerializer.Deserialize<IEnumerable<TodoItemDescription>>(await response2.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true}).Single(); 

            // Act
            var response = await _webClient.PutAsync($"/TodoItem?id={todoItem.Id}", null);
            response.IsSuccessStatusCode.Should().BeTrue();
            await Task.Delay(WaitDuration);

            // Assert
            _signalRPublisherMock.Verify(x => x.RemoveTodoItemdoItem(It.Is<Guid>(x => x == todoItem.Id)), Times.Once);

            await AssertHangfireJobs(total: 2, succeeded: 2);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }

        [TestMethod]
        public async Task CompleteTodoItem_WhenExceptionRemoveTodoCommandOccurs_ThenNoSignalRInvokedAndTodoItemNotRemoved()
        {
            // Arrange
            var response1 = await _webClient.PostAsync("/TodoItem?label=test", null);
            response1.IsSuccessStatusCode.Should().BeTrue();

            var response2 = await _webClient.GetAsync("/TodoItem");
            response2.IsSuccessStatusCode.Should().BeTrue();
            var todoItem = JsonSerializer.Deserialize<IEnumerable<TodoItemDescription>>(await response2.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true}).Single();

            _chaosMonkeyMock.Setup(x => x.OnNsbHandle()).Throws<Exception>();

            // Act
            var response = await _webClient.PutAsync($"/TodoItem?id={todoItem.Id}", null);
            response.IsSuccessStatusCode.Should().BeTrue();
            await Task.Delay(WaitDuration);

            // Assert
            _signalRPublisherMock.Verify(x => x.RemoveTodoItemdoItem(It.Is<Guid>(x => x == todoItem.Id)), Times.Never);
            using var scope = _web.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Single(i => i.Id == todoItem.Id).Removed.Should().BeFalse();

            await AssertHangfireJobs(total: 2, succeeded: 2);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 1);
        }

        [TestMethod]
        public async Task CompleteTodoItem_WhenExceptionBecauseOfRemovedUniqueConstraint_ThenNoSignalRInvokedAndTodoItemNotRemoved()
        {
            // Arrange
            var response1 = await _webClient.PostAsync("/TodoItem?label=test", null);
            response1.IsSuccessStatusCode.Should().BeTrue();

            var response2 = await _webClient.GetAsync("/TodoItem");
            response2.IsSuccessStatusCode.Should().BeTrue();
            var todoItem1 = JsonSerializer.Deserialize<IEnumerable<TodoItemDescription>>(await response2.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true}).Single();

            var response3 = await _webClient.PutAsync($"/TodoItem?id={todoItem1.Id}", null);
            response3.IsSuccessStatusCode.Should().BeTrue();
            await Task.Delay(WaitDuration);

            var response4 = await _webClient.PostAsync("/TodoItem?label=test2", null);
            response4.IsSuccessStatusCode.Should().BeTrue();

            var response5 = await _webClient.GetAsync("/TodoItem");
            response5.IsSuccessStatusCode.Should().BeTrue();
            var todoItem2 = JsonSerializer.Deserialize<IEnumerable<TodoItemDescription>>(await response5.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true}).Single(x => x.Label == "test2");

            // Act
            var response = await _webClient.PutAsync($"/TodoItem?id={todoItem2.Id}", null);
            response.IsSuccessStatusCode.Should().BeTrue();
            await Task.Delay(WaitDuration);

            // Assert
            _signalRPublisherMock.Verify(x => x.RemoveTodoItemdoItem(It.Is<Guid>(x => x == todoItem1.Id)), Times.Once);
            _signalRPublisherMock.Verify(x => x.RemoveTodoItemdoItem(It.Is<Guid>(x => x == todoItem2.Id)), Times.Never);
            using var scope = _web.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Single(i => i.Id == todoItem1.Id).Removed.Should().BeTrue();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Single(i => i.Id == todoItem2.Id).Removed.Should().BeFalse();

            await AssertHangfireJobs(total: 4, succeeded: 4);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 1);
        }

        [TestMethod]
        public async Task DeleteRemovedTodoItems_WhenNoException_ThenSignalRInvoked()
        {
            // Arrange
            var response1 = await _webClient.PostAsync("/TodoItem?label=test", null);
            response1.IsSuccessStatusCode.Should().BeTrue();

            var response2 = await _webClient.GetAsync("/TodoItem");
            response2.IsSuccessStatusCode.Should().BeTrue();
            var todoItem1 = JsonSerializer.Deserialize<IEnumerable<TodoItemDescription>>(await response2.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true}).Single();

            var response3 = await _webClient.PutAsync($"/TodoItem?id={todoItem1.Id}", null);
            response3.IsSuccessStatusCode.Should().BeTrue();
            await Task.Delay(WaitDuration);

            // Act
            using var actScope = _worker.Services.CreateScope();
            actScope.ServiceProvider.GetRequiredService<IRecurringJobManager>().Trigger(nameof(DeleteRemovedTodoItemsCommand));
            await Task.Delay(WaitDuration);

            // Assert
            _signalRPublisherMock.Verify(x => x.TodoItemsDeleted(), Times.Once);
            using var scope = _web.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Should().HaveCount(0);

            await AssertHangfireJobs(total: 4, succeeded: 4);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }

        [TestMethod]
        public async Task DeleteRemovedTodoItems_WhenException_ThenNoSignalRInvoked()
        {
            // Arrange
            var response1 = await _webClient.PostAsync("/TodoItem?label=test", null);
            response1.IsSuccessStatusCode.Should().BeTrue();

            var response2 = await _webClient.GetAsync("/TodoItem");
            response2.IsSuccessStatusCode.Should().BeTrue();
            var todoItem1 = JsonSerializer.Deserialize<IEnumerable<TodoItemDescription>>(await response2.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true}).Single();

            var response3 = await _webClient.PutAsync($"/TodoItem?id={todoItem1.Id}", null);
            response3.IsSuccessStatusCode.Should().BeTrue();
            await Task.Delay(WaitDuration);

            _chaosMonkeyMock.Setup(x => x.OnUowCommit()).Throws<Exception>();

            // Act
            using var actScope = _worker.Services.CreateScope();
            actScope.ServiceProvider.GetRequiredService<IRecurringJobManager>().Trigger(nameof(DeleteRemovedTodoItemsCommand));
            await Task.Delay(WaitDuration);

            // Assert
            _signalRPublisherMock.Verify(x => x.TodoItemsDeleted(), Times.Never);
            using var scope = _web.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Should().HaveCount(1);

            await AssertHangfireJobs(total: 3, succeeded: 2);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }

        private async Task PurgeRabbitMq()
        {
            await PurgeRabbitMqQueue(WorkerQueue);
            await PurgeRabbitMqQueue(WebQueue);
            await PurgeRabbitMqQueue(ErrorQueue);
        }

        private async Task PurgeRabbitMqQueue(string queueName)
        {
            var vhost = await _rabbitMqClient.GetVhostAsync("/");
            var queue = await _rabbitMqClient.GetQueueAsync(queueName, vhost);
            await _rabbitMqClient.PurgeAsync(queue);
        }

        private async Task PurgeDb()
        {
            using var scope = _web.Services.CreateScope();
            var db = scope.ServiceProvider.GetService<ReadModelDbContext>();

            await db.Database.ExecuteSqlRawAsync(@"
TRUNCATE TABLE [dbo].[NSB_OutboxData]
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

        private async Task AssertRabbitMqQueueLength(string queueName, int count)
        {
            var vhost = await _rabbitMqClient.GetVhostAsync("/");
            var queue = await _rabbitMqClient.GetQueueAsync(queueName, vhost);
            var messages = await _rabbitMqClient.GetMessagesFromQueueAsync(queue, new GetMessagesCriteria(100, Ackmodes.ack_requeue_true));

            messages.Should().HaveCount(count, "Found messages in queue '{0}': {1}", queueName, JsonSerializer.Serialize(messages));
        }

        private async Task AssertHangfireJobs(int total, int succeeded)
        {
            using var scope = _web.Services.CreateScope();
            var jobs = await scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().Job.ToListAsync();
            jobs.Should().HaveCount(total);
            jobs.Where(j => j.StateName == "Succeeded").Should().HaveCount(succeeded);
            scope.Dispose();
        }
    }
}
