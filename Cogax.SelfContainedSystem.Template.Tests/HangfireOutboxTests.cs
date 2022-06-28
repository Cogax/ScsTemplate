using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Commands;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Readmodels;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;
using Cogax.SelfContainedSystem.Template.Tests.Utils;

using FluentAssertions;

using Hangfire;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Cogax.SelfContainedSystem.Template.Tests
{
    [TestClass]
    public class HangfireOutboxTests : WebApplicationTestBase
    {
        [TestMethod]
        public async Task CreateTodoItem_WhenNoException_ThenSignalRInvoked()
        {
            // Arrange
            // Act
            await WebClient.PostAsync("/TodoItem?label=test", null);
            await Task.Delay(WaitDuration);

            // Assert
            SignalRPublisherMock.Verify(x => x.NewTodoItem(It.Is<TodoItemDescription>(d => d.Label == "test")), Times.Once);

            await AssertHangfireJobs(total: 1, succeeded: 1);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }

        [TestMethod]
        public async Task CreateTodoItem_WhenExceptionBeforeCommitOccurs_ThenNoSignalRInvoked()
        {
            // Arrange
            ChaosMonkeyMock.Setup(x => x.OnUowCommit()).Throws<Exception>();

            // Act
            var response = await WebClient.PostAsync("/TodoItem?label=test", null);
            response.IsSuccessStatusCode.Should().BeFalse();
            await Task.Delay(WaitDuration);

            // Assert
            SignalRPublisherMock.Verify(x => x.NewTodoItem(It.Is<TodoItemDescription?>(d => d == null)), Times.Never);

            await AssertHangfireJobs(total: 0, succeeded: 0);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }

        [TestMethod]
        public async Task CreateTodoItem_WhenExceptionBecauseOfLabelUniqueConstraint_ThenNoSignalRInvoked()
        {
            // Arrange
            var response1 = await WebClient.PostAsync("/TodoItem?label=test", null);
            response1.IsSuccessStatusCode.Should().BeTrue();

            // Act
            var response = await WebClient.PostAsync("/TodoItem?label=test", null);
            response.IsSuccessStatusCode.Should().BeFalse();
            await Task.Delay(WaitDuration);

            // Assert
            SignalRPublisherMock.Verify(x => x.NewTodoItem(It.IsAny<TodoItemDescription>()), Times.Once);

            await AssertHangfireJobs(total: 1, succeeded: 1);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }
        
        [TestMethod]
        public async Task DeleteRemovedTodoItems_WhenNoException_ThenSignalRInvoked()
        {
            // Arrange
            var response1 = await WebClient.PostAsync("/TodoItem?label=test", null);
            response1.IsSuccessStatusCode.Should().BeTrue();

            var response2 = await WebClient.GetAsync("/TodoItem");
            response2.IsSuccessStatusCode.Should().BeTrue();
            var todoItem1 = JsonSerializer.Deserialize<IEnumerable<TodoItemDescription>>(await response2.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true}).Single();

            var response3 = await WebClient.PutAsync($"/TodoItem?id={todoItem1.Id}", null);
            response3.IsSuccessStatusCode.Should().BeTrue();
            await Task.Delay(WaitDuration);

            // Act
            using var actScope = Worker.Services.CreateScope();
            actScope.ServiceProvider.GetRequiredService<IRecurringJobManager>().Trigger(nameof(DeleteRemovedTodoItemsCommand));
            await Task.Delay(WaitDuration);

            // Assert
            SignalRPublisherMock.Verify(x => x.TodoItemsDeleted(), Times.Once);
            using var scope = Web.Services.CreateScope();
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
            var response1 = await WebClient.PostAsync("/TodoItem?label=test", null);
            response1.IsSuccessStatusCode.Should().BeTrue();

            var response2 = await WebClient.GetAsync("/TodoItem");
            response2.IsSuccessStatusCode.Should().BeTrue();
            var todoItem1 = JsonSerializer.Deserialize<IEnumerable<TodoItemDescription>>(await response2.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true}).Single();

            var response3 = await WebClient.PutAsync($"/TodoItem?id={todoItem1.Id}", null);
            response3.IsSuccessStatusCode.Should().BeTrue();
            await Task.Delay(WaitDuration);

            ChaosMonkeyMock.Setup(x => x.OnUowCommit()).Throws<Exception>();

            // Act
            using var actScope = Worker.Services.CreateScope();
            actScope.ServiceProvider.GetRequiredService<IRecurringJobManager>().Trigger(nameof(DeleteRemovedTodoItemsCommand));
            await Task.Delay(WaitDuration);

            // Assert
            SignalRPublisherMock.Verify(x => x.TodoItemsDeleted(), Times.Never);
            using var scope = Web.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Should().HaveCount(1);

            await AssertHangfireJobs(total: 3, succeeded: 2);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }
    }
}
