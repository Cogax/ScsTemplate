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
            using var scope = Web.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Should().HaveCount(1);
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Single().Label.Should().Be("test");

            await AssertHangfireJobs(total: 2, succeeded: 2);
            await AssertNsbInboxOutboxEntries(total: 1);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }

        [TestMethod]
        public async Task CreateTodoItem_WhenExceptionBeforeCommitOccurs_ThenNoSignalRInvoked()
        {
            // Arrange
            ChaosMonkeyMock.Setup(x => x.AtEndOfUnitOfWorkScope()).Throws<Exception>();

            // Act
            var response = await WebClient.PostAsync("/TodoItem?label=test", null);
            response.IsSuccessStatusCode.Should().BeFalse();
            await Task.Delay(WaitDuration);

            // Assert
            SignalRPublisherMock.Verify(x => x.NewTodoItem(It.IsAny<TodoItemDescription?>()), Times.Never);
            using var scope = Web.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Should().HaveCount(0);

            await AssertHangfireJobs(total: 0, succeeded: 0);
            await AssertNsbInboxOutboxEntries(total: 0);
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
            using var scope = Web.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Should().HaveCount(1);

            await AssertHangfireJobs(total: 2, succeeded: 2);
            await AssertNsbInboxOutboxEntries(total: 1);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }

        [TestMethod]
        public async Task CreateTodoItem_WhenExceptionBeforeSignalRPublishJobEnqueued_ThenNoSignalRInvoked()
        {
            // Arrange
            ChaosMonkeyMock.Setup(x => x.OnNsbHandleTodoItemAdded()).Throws<Exception>();

            // Act
            var response = await WebClient.PostAsync("/TodoItem?label=test", null);
            response.IsSuccessStatusCode.Should().BeTrue();
            await Task.Delay(WaitDuration);

            // Assert
            SignalRPublisherMock.Verify(x => x.NewTodoItem(It.IsAny<TodoItemDescription?>()), Times.Never);
            using var scope = Web.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Should().HaveCount(1);

            await AssertHangfireJobs(total: 1, succeeded: 1);
            await AssertNsbInboxOutboxEntries(total: 0);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 1);
        }

         [TestMethod]
        public async Task CompleteTodoItem_WhenNoException_ThenSignalRInvoked()
        {
            // Arrange
            var response1 = await WebClient.PostAsync("/TodoItem?label=test", null);
            response1.IsSuccessStatusCode.Should().BeTrue();

            var response2 = await WebClient.GetAsync("/TodoItem");
            response2.IsSuccessStatusCode.Should().BeTrue();
            var todoItem = JsonSerializer.Deserialize<IEnumerable<TodoItemDescription>>(await response2.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true}).Single(); 

            // Act
            var response = await WebClient.PutAsync($"/TodoItem?id={todoItem.Id}", null);
            response.IsSuccessStatusCode.Should().BeTrue();
            await Task.Delay(WaitDuration);

            // Assert
            SignalRPublisherMock.Verify(x => x.RemoveTodoItemdoItem(It.Is<Guid>(x => x == todoItem.Id)), Times.Once);
            using var scope = Web.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Should().HaveCount(1);
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Single().Completed.Should().BeTrue();

            await AssertHangfireJobs(total: 5, succeeded: 5);
            await AssertNsbInboxOutboxEntries(total: 3);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }

        [TestMethod]
        public async Task CompleteTodoItem_WhenExceptionRemoveTodoCommandOccurs_ThenNoSignalRInvokedAndTodoItemNotRemoved()
        {
            // Arrange
            var response1 = await WebClient.PostAsync("/TodoItem?label=test", null);
            response1.IsSuccessStatusCode.Should().BeTrue();

            var response2 = await WebClient.GetAsync("/TodoItem");
            response2.IsSuccessStatusCode.Should().BeTrue();
            var todoItem = JsonSerializer.Deserialize<IEnumerable<TodoItemDescription>>(await response2.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true}).Single();

            ChaosMonkeyMock.Setup(x => x.OnNsbHandleTodoItemCompleted()).Throws<Exception>();

            // Act
            var response = await WebClient.PutAsync($"/TodoItem?id={todoItem.Id}", null);
            response.IsSuccessStatusCode.Should().BeTrue();
            await Task.Delay(WaitDuration);

            // Assert
            SignalRPublisherMock.Verify(x => x.RemoveTodoItemdoItem(It.Is<Guid>(x => x == todoItem.Id)), Times.Never);
            using var scope = Web.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Single(i => i.Id == todoItem.Id).Removed.Should().BeFalse();

            await AssertHangfireJobs(total: 3, succeeded: 3);
            await AssertNsbInboxOutboxEntries(total: 1);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 1);
        }

        [TestMethod]
        public async Task CompleteTodoItem_WhenExceptionBecauseOfRemovedUniqueConstraint_ThenNoSignalRInvokedAndTodoItemNotRemoved()
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

            var response4 = await WebClient.PostAsync("/TodoItem?label=test2", null);
            response4.IsSuccessStatusCode.Should().BeTrue();

            var response5 = await WebClient.GetAsync("/TodoItem");
            response5.IsSuccessStatusCode.Should().BeTrue();
            var todoItem2 = JsonSerializer.Deserialize<IEnumerable<TodoItemDescription>>(await response5.Content.ReadAsStringAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true}).Single(x => x.Label == "test2");

            // Act
            var response = await WebClient.PutAsync($"/TodoItem?id={todoItem2.Id}", null);
            response.IsSuccessStatusCode.Should().BeTrue();
            await Task.Delay(WaitDuration);

            // Assert
            SignalRPublisherMock.Verify(x => x.RemoveTodoItemdoItem(It.Is<Guid>(x => x == todoItem1.Id)), Times.Once);
            SignalRPublisherMock.Verify(x => x.RemoveTodoItemdoItem(It.Is<Guid>(x => x == todoItem2.Id)), Times.Never);
            using var scope = Web.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Single(i => i.Id == todoItem1.Id).Removed.Should().BeTrue();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Single(i => i.Id == todoItem2.Id).Removed.Should().BeFalse();

            await AssertHangfireJobs(total: 8, succeeded: 8);
            await AssertNsbInboxOutboxEntries(total: 4);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 1);
        }

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

            await AssertHangfireJobs(total: 8, succeeded: 8);
            await AssertNsbInboxOutboxEntries(total: 4);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }

        [TestMethod]
        public async Task DeleteRemovedTodoItems_WhenExceptionInsideOfUnitOfWork_ThenNoSignalRInvoked()
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

            ChaosMonkeyMock.Setup(x => x.AtEndOfUnitOfWorkScope()).Throws<Exception>();

            // Act
            using var actScope = Worker.Services.CreateScope();
            actScope.ServiceProvider.GetRequiredService<IRecurringJobManager>().Trigger(nameof(DeleteRemovedTodoItemsCommand));
            await Task.Delay(WaitDuration);

            // Assert
            SignalRPublisherMock.Verify(x => x.TodoItemsDeleted(), Times.Never);
            using var scope = Web.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Should().HaveCount(1);

            await AssertHangfireJobs(total: 6, succeeded: 5);
            await AssertNsbInboxOutboxEntries(total: 3);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }

        [TestMethod, Ignore("Hangfire Jobs have to be idempotent: https://docs.hangfire.io/en/latest/best-practices.html")]
        public async Task DeleteRemovedTodoItems_WhenExceptionAfterUnitOfWork_ThenNoSignalRInvoked()
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

            ChaosMonkeyMock.Setup(x => x.AfterUnitOfWorkCommitted()).Throws<Exception>();

            // Act
            using var actScope = Worker.Services.CreateScope();
            actScope.ServiceProvider.GetRequiredService<IRecurringJobManager>().Trigger(nameof(DeleteRemovedTodoItemsCommand));
            await Task.Delay(WaitDuration);

            // Assert
            //SignalRPublisherMock.Verify(x => x.TodoItemsDeleted(), Times.Never);
            using var scope = Web.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ReadModelDbContext>().TodoItems.Should().HaveCount(1);

            await AssertHangfireJobs(total: 6, succeeded: 5);
            await AssertNsbInboxOutboxEntries(total: 3);
            await AssertRabbitMqQueueLength(WebQueue, 0);
            await AssertRabbitMqQueueLength(WorkerQueue, 0);
            await AssertRabbitMqQueueLength(ErrorQueue, 0);
        }
    }
}
