using Application.Dtos.Common;
using Application.Dtos.CurrencyRate;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers.v1;

namespace WebApi.Tests.Controllers.v1;

public class CurrencyRatesControllerTests
{
    private readonly Mock<ICurrencyRateProvider> _currencyProviderMock = new();

    private CurrencyRatesController CreateController()
    {
        return new CurrencyRatesController(_currencyProviderMock.Object);
    }

    [Fact]
    public async Task GetAsync_ReturnsOk_WithLatestResult()
    {
        // Arrange
        var controller = CreateController();
        var request = new LatestRequest("EUR", ["USD"]);

        var expectedResult = new LatestResult(
            Base: "EUR",
            Date: DateTime.UtcNow.Date,
            Rates: new Dictionary<string, double> { { "USD", 1.5 } }
        );

        _currencyProviderMock
            .Setup(p => p.GetLatestCurrencyRatesAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await controller.GetAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<LatestResult>(okResult.Value);
        Assert.Equal(expectedResult.Base, value.Base);
        Assert.Equal(expectedResult.Rates["USD"], value.Rates["USD"]);
    }

    [Fact]
    public async Task ConvertAsync_ReturnsOk_WithConvertResult()
    {
        // Arrange
        var controller = CreateController();
        var request = new ConvertRequest("USD", "EUR", 100);

        var expectedResult = new ConvertResult(
            Base: "USD",
            Target: "EUR",
            Amount: 100,
            Result: 85,
            Rate: 0.85,
            Date: DateTime.UtcNow
        );

        _currencyProviderMock
            .Setup(p => p.ConvertCurrencyRatesAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await controller.ConvertAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<ConvertResult>(okResult.Value);
        Assert.Equal(expectedResult.Result, value.Result);
    }

    [Fact]
    public async Task GetHistoricalAsync_ReturnsOk_WithPagedResult()
    {
        // Arrange
        var controller = CreateController();
        var request = new HistoricalRequest(
            StartDate: DateTime.UtcNow.AddDays(-1),
            Base: "USD",
            Symbols: ["EUR"]
        )
        {
            EndDate = DateTime.UtcNow
        };

        var historicalRecords = new List<HistoricalResult>
        {
            new(
                DateTime.UtcNow.AddDays(-1),
                new Dictionary<string, double> { { "EUR", 0.85 } }
            ),
            new(
                DateTime.UtcNow,
                new Dictionary<string, double> { { "EUR", 0.86 } }
            )
        };

        var expectedResult = new PagedResult<HistoricalResult>(historicalRecords, historicalRecords.Count);

        _currencyProviderMock
            .Setup(p => p.GetHistoricalCurrencyRatesAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await controller.GetHistoricalAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PagedResult<HistoricalResult>>(okResult.Value);
        Assert.Equal(expectedResult.TotalRecords, value.TotalRecords);
        Assert.Equal(expectedResult.Records!.Count(), value.Records!.Count());
    }
}
