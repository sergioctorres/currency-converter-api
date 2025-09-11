using Application.Dtos.CurrencyRate;
using Infrastructure.Currency.Providers;
using Moq.Protected;
using Moq;
using System.Net;
using System.Text;

namespace Infrastructure.Tests.Currency.Providers;

public class FrankfurterCurrencyProviderTests
{
    private static FrankfurterCurrencyProvider CreateProvider(HttpResponseMessage responseMessage)
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        var clientFactory = new Mock<IHttpClientFactory>();

        clientFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(handler.Object)
            {
                BaseAddress = new Uri("https://api.frankfurter.dev/v1")
            });

        return new FrankfurterCurrencyProvider(clientFactory.Object);
    }

    [Fact]
    public async Task GetLatestCurrencyRatesAsync_WithValidResponse_ReturnsLatestResult()
    {
        // Arrange
        var json =
            """
            { "base":"USD", "date":"2025-09-01", "rates": { "EUR":0.85 } }
            """;

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var provider = CreateProvider(response);

        // Act
        var result = await provider.GetLatestCurrencyRatesAsync(new LatestRequest("USD", ["EUR"]));

        // Assert
        Assert.Equal("USD", result!.Base);
        Assert.Equal(0.85, result.Rates["EUR"]);
    }

    [Fact]
    public async Task GetLatestCurrencyRatesAsync_WithApiFailure_ReturnsNull()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var provider = CreateProvider(response);

        // Act
        var result = await provider.GetLatestCurrencyRatesAsync(new LatestRequest("USD", ["EUR"]));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetHistoricalCurrencyRatesAsync_WithValidResponse_ReturnsPagedResult()
    {
        // Arrange
        var json =
            """
            {
              "base": "USD",
              "start_date": "2025-09-01",
              "end_date": "2025-09-03",
              "rates": {
                "2025-09-01": { "EUR": 0.85 },
                "2025-09-02": { "EUR": 0.86 },
                "2025-09-03": { "EUR": 0.87 }
              }
            }
            """;

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var provider = CreateProvider(response);

        var request = new HistoricalRequest(
            StartDate: new DateTime(2025, 9, 1),
            Base: "USD",
            Symbols: ["EUR"]
        )
        { EndDate = new DateTime(2025, 9, 3) };

        // Act
        var result = await provider.GetHistoricalCurrencyRatesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result!.Records!.Count());
        Assert.Equal("EUR", result!.Records!.First().Values.Keys.First());
    }
}
