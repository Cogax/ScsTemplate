using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Readmodels;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.Persistence.DbContexts;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.SignalR;
using Cogax.SelfContainedSystem.Template.Tests.Utils;
using Cogax.SelfContainedSystem.Template.Web;
using Cogax.SelfContainedSystem.Template.Worker;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Cogax.SelfContainedSystem.Template.Tests
{
    [TestClass]
    public class OutboxConsistencyTests
    {
        private static TimeSpan WaitDuration = TimeSpan.FromSeconds(5);
        private TestableWebApplication<WebProgram> _web = null!;
        private TestableWebApplication<WorkerProgram> _worker = null!;
        private HttpClient _webClient = null!;
        private HttpClient _workerClient = null!;

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

            _webClient = _web.CreateClient();
            _workerClient = _worker.CreateClient();

            await _webClient.DeleteAsync("/TodoItem");
        }

        [TestCleanup]
        public async Task TearDown()
        {
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
            _signalRPublisherMock.Verify(x => x.NewTodoItem(It.IsAny<TodoItemDescription>()), Times.Once);
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
            _signalRPublisherMock.Verify(x => x.NewTodoItem(It.IsAny<TodoItemDescription>()), Times.Never);
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
        }
    }
}
