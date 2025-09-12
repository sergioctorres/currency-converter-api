using Polly;
using WebApi.Configuration.HttpCommunication.Constants;

namespace WebApi.Configuration.HttpCommunication;

public static class HttpCommunicationExtensions
{
    public static IHttpClientBuilder AddHttpCommunication(this IServiceCollection services, string httpClientName, string httpClientBaseAddress)
    {
        return services.AddHttpClient(httpClientName, delegate (HttpClient client)
        {
            client.BaseAddress = new Uri(httpClientBaseAddress);

            return;
        })
        .AddTransientHttpErrorPolicy(builder =>
            builder.WaitAndRetryAsync(HttpConstants.DefaultRetryTimings))
        .AddTransientHttpErrorPolicy(builder =>
            builder.CircuitBreakerAsync(
                HttpConstants.HandledEventsAllowedBeforeBreaking,
                HttpConstants.DurationOfBreak
            )
        );
    }
}
