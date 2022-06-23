using System;
using System.Net.Http;
using System.Threading.Tasks;

using Cogax.SelfContainedSystem.Template.Core.Application.Common.Consistency;
using Cogax.SelfContainedSystem.Template.Core.Application.Todo.Ports;
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
    public class AddTodoItemTests
    {
        private WebFactory _web = null!;
        private WorkerFactory _worker = null!;
        private HttpClient _webClient = null!;
        private HttpClient _workerClient = null!;

        private Mock<ISignalRPublisher> _signalRPublisherMock = null!;
        private Mock<ITodoEmailPort> _emailPortMock = null!;
        private Mock<IChaosMonkey> _chaosMonkeyMock = null!;

        [TestInitialize]
        public async Task SetUp()
        {
            _signalRPublisherMock = new Mock<ISignalRPublisher>();
            _emailPortMock = new Mock<ITodoEmailPort>();
            _chaosMonkeyMock = new Mock<IChaosMonkey>();

            Action<IServiceCollection> servicesOverride = (services) =>
            {
                services.AddSingleton<ISignalRPublisher>(_signalRPublisherMock.Object);
                services.AddSingleton<ITodoEmailPort>(_emailPortMock.Object);
                services.AddSingleton<IChaosMonkey>(_chaosMonkeyMock.Object);
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
        public async Task WhenNoExceptionBeforeCommit_ThenEventIsPublished()
        {
            // Arrange
            // Act
            await _webClient.PostAsync("/TodoItem?label=test", null);
            await Task.Delay(TimeSpan.FromSeconds(5));

            // Assert
            _signalRPublisherMock.Verify(x => x.PublishTodoItem(It.IsAny<TodoItemDescription>()), Times.Once);
        }

        [TestMethod]
        public async Task WhenExceptionBeforeCommitOccurs_ThenNoEventIsPublished()
        {
            // Arrange
            _chaosMonkeyMock.Setup(x => x.OnUowCommit()).Throws<Exception>();

            // Act
            var response = await _webClient.PostAsync("/TodoItem?label=test", null);
            await Task.Delay(TimeSpan.FromSeconds(5));

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();
            _signalRPublisherMock.Verify(x => x.PublishTodoItem(It.IsAny<TodoItemDescription>()), Times.Never);
        }
    }
}
