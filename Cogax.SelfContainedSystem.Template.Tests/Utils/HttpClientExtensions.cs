using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cogax.SelfContainedSystem.Template.Tests.Utils;

public static class HttpClientExtensions
{
    public static async Task<T?> GetAsync<T>(this HttpClient httpClient, string? requestUri)
    {
        var response = await httpClient.GetAsync(requestUri);
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json);
    }
}
