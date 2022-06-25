using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Readmodels;
using Cogax.SelfContainedSystem.Template.Infrastructure.Adapters.SignalR;
using Cogax.SelfContainedSystem.Template.Tests.Utils;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Cogax.SelfContainedSystem.Template.Tests
{
    [TestClass]
    public class OutboxConsistencyTests
    {
        private WebFactory _web = null!;
        private WorkerFactory _worker = null!;
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

            _web = new WebFactory(servicesOverride);
            _worker = new WorkerFactory(servicesOverride);

            _webClient = _web.CreateClient();
            _workerClient = _worker.CreateClient();
            
            await _webClient.DeleteAsync("/TodoItem");
        }

        [TestCleanup]
        public async Task CleanUp()
        {
            _webClient?.Dispose();
            _workerClient?.Dispose();
            await _web.DisposeAsync();
            await _worker.DisposeAsync();
        }

        [TestMethod]
        public async Task CreateTodoItem_WhenNoException_ThenSignalRPublishesNewTodoItem()
        {
            // Arrange
            // Act
            await _webClient.PostAsync("/TodoItem?label=test", null);
            await Task.Delay(TimeSpan.FromSeconds(5));

            // Assert
            _signalRPublisherMock.Verify(x => x.NewTodoItem(It.IsAny<TodoItemDescription>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateTodoItem_WhenExceptionBeforeCommitOccurs_ThenNoSignalRPublish()
        {
            // Arrange
            _chaosMonkeyMock.Setup(x => x.OnUowCommit()).Throws<Exception>();

            // Act
            var response = await _webClient.PostAsync("/TodoItem?label=test", null);
            response.IsSuccessStatusCode.Should().BeFalse();
            await Task.Delay(TimeSpan.FromSeconds(5));

            // Assert
            _signalRPublisherMock.Verify(x => x.NewTodoItem(It.IsAny<TodoItemDescription>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateTodoItem_WhenExceptionBecauseOfUniqueConstraint_ThenNoSignalRPublish()
        {
            // Arrange
            var response1 = await _webClient.PostAsync("/TodoItem?label=test", null);
            response1.IsSuccessStatusCode.Should().BeTrue();

            // Act
            var response = await _webClient.PostAsync("/TodoItem?label=test", null);
            response.IsSuccessStatusCode.Should().BeFalse();
            await Task.Delay(TimeSpan.FromSeconds(5));

            // Assert
            _signalRPublisherMock.Verify(x => x.NewTodoItem(It.IsAny<TodoItemDescription>()), Times.Once);
        }

        [TestMethod]
        public async Task CompleteTodoItem_WhenNoException_ThenSignalRPublishesRemovedTodoItem()
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
            await Task.Delay(TimeSpan.FromSeconds(5));

            // Assert
            _signalRPublisherMock.Verify(x => x.RemoveTodoItemdoItem(It.Is<Guid>(x => x == todoItem.Id)), Times.Once);
        }
    }
}
