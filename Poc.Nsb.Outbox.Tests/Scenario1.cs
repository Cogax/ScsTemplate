using System.Net.Http;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Poc.Nsb.Outbox.Core;


namespace Poc.Nsb.Outbox.Tests
{
    [TestClass]
    public class Scenario1
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
            (await client.GetAsync<MyEntity[]>($"{WebUrl}/MyEntity")).Should().BeEmpty();
        }

        [TestMethod]
        public async Task WennExceptionVorSave_DannSollKeinEventGesendetWerden()
        {
            var response = await client.PostAsync($"{WebUrl}/MyEntity?foo=test&exception=true", null);
            response.IsSuccessStatusCode.Should().BeFalse();
            (await client.GetAsync<object[]>($"{WorkerUrl}/Store")).Should().BeEmpty();
        }
    }
}
