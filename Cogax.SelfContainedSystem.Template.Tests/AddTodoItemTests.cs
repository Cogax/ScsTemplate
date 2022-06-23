using System.Net.Http;
using System.Threading.Tasks;

using Cogax.SelfContainedSystem.Template.Core.Domain.Todo.Aggregates;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cogax.SelfContainedSystem.Template.Tests
{
    [TestClass]
    public class AddTodoItemTests
    {
        private const string WorkerUrl = "http://localhost:5267";
        private const string WebUrl = "http://localhost:5086";
        private readonly HttpClient client = new();

        [TestInitialize]
        public async Task SetUp()
        {
            await client.DeleteAsync($"{WorkerUrl}/Store");
            (await client.GetAsync<object[]>($"{WorkerUrl}/Store")).Should().BeEmpty();

            await client.DeleteAsync($"{WebUrl}/MyEntity");
            (await client.GetAsync<TodoItem[]>($"{WebUrl}/MyEntity")).Should().BeEmpty();
        }

        [TestMethod]
        public async Task WhenNoExceptionBeforeCommit_ThenEventIsPublished()
        {
            var response = await client.PostAsync($"{WebUrl}/MyEntity?foo=test&exception=false", null);
            response.IsSuccessStatusCode.Should().BeTrue();
            (await client.GetAsync<object[]>($"{WorkerUrl}/Store")).Should().HaveCount(1);
        }

        [TestMethod]
        public async Task WhenExceptionBeforeCommitOccurs_ThenNoEventIsPublished()
        {
            var response = await client.PostAsync($"{WebUrl}/MyEntity?foo=test&exception=true", null);
            response.IsSuccessStatusCode.Should().BeFalse();
            (await client.GetAsync<object[]>($"{WorkerUrl}/Store")).Should().BeEmpty();
        }
    }
}
