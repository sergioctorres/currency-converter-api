using System.Net.Http.Json;

namespace Infrastructure.Http;

public abstract class HttpClientBase(IHttpClientFactory httpClientFactory)
{
    protected abstract string GetClientName();

    protected async Task<T?> SendAsync<T>(HttpMethod method, string url, CancellationToken cancellationToken = default)
    {
        using var client = httpClientFactory.CreateClient(GetClientName());

        var request = new HttpRequestMessage
        {
            Method = method,
            RequestUri = new Uri(client.BaseAddress + url)
        };

        var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
    }
}
