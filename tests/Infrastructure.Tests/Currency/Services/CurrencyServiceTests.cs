using Application.Dtos.Common;
using Application.Dtos.CurrencyRate;
using Application.Interfaces;
using Infrastructure.Currency.Configurations;
using Infrastructure.Currency.Constants;
using Infrastructure.Currency.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace Infrastructure.Tests.Currency.Services;

public class CurrencyServiceTests
{
    private readonly Mock<ICurrencyRateProvider> _providerMock = new();
    private readonly CurrencyService _service;

    public CurrencyServiceTests()
    {
        var factoryMock = new Mock<ICurrencyRateProviderFactory>();
        var config = Options.Create(new CurrencyProviderConfiguration { Name = CurrencyRateConstants.Providers.FrankfurterApi });

        factoryMock.Setup(f => f.Create(config.Value.Name)).Returns(_providerMock.Object);
        _service = new CurrencyService(factoryMock.Object, config);
    }

    [Fact]
    public async Task GetLatestCurrencyRatesAsync_RemovesBlockedCurrencies()
    {
        // Arrange
        var request = new LatestRequest("USD", ["EUR", "TRY"]);
        var original = new LatestResult(
            Base: "USD",
            Date: DateTime.UtcNow,
            Rates: new Dictionary<string, double>
            {
                { "EUR", 0.85 },
                { "TRY", 32.5 }
            });

        _providerMock.Setup(p => p.GetLatestCurrencyRatesAsync(request, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(original);

        // Act
        var result = await _service.GetLatestCurrencyRatesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result!.Rates);
        Assert.True(result.Rates.ContainsKey("EUR"));
        Assert.False(result.Rates.ContainsKey("TRY"));
    }

    [Fact]
    public async Task GetHistoricalCurrencyRatesAsync_RemovesBlockedCurrencies()
    {
        // Arrange
        var request = new HistoricalRequest(DateTime.UtcNow.AddDays(-2)) { EndDate = DateTime.UtcNow };

        var records = new List<HistoricalResult>
        {
            new(
                DateTime.UtcNow.AddDays(-2),
                new Dictionary<string, double> { { "EUR", 0.85 }, { "MXN", 18.2 }, { "PLN", 4.5 } }),
            new(
                DateTime.UtcNow.AddDays(-1),
                new Dictionary<string, double> { { "USD", 1.0 }, { "MXN", 18.4 }, { "PLN", 4.6 } })
        };

        var paged = new PagedResult<HistoricalResult>(records, records.Count);

        _providerMock.Setup(p => p.GetHistoricalCurrencyRatesAsync(request, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(paged);

        // Act
        var result = await _service.GetHistoricalCurrencyRatesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result!.Records!.Count());
        Assert.Single(result.Records!.First().Values);
        Assert.Single(result.Records!.Last().Values);
    }

    [Fact]
    public async Task ConvertCurrencyRatesAsync_DelegatesToProvider()
    {
        // Arrange
        var request = new ConvertRequest("USD", "EUR", 100);
        var expected = new ConvertResult("USD", "EUR", 100, 85, 0.85, DateTime.UtcNow);

        _providerMock.Setup(p => p.ConvertCurrencyRatesAsync(request, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected);

        // Act
        var result = await _service.ConvertCurrencyRatesAsync(request);

        // Assert
        Assert.Equal(expected.Result, result!.Result);
        _providerMock.Verify(p => p.ConvertCurrencyRatesAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}
