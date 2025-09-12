using Infrastructure.Redis;

namespace Infrastructure.Tests.Redis;

using System.Text;
using System.Text.Json;
using Application.Dtos.CurrencyRate;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

public class RedisCacheServiceTests
{
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly RedisCacheService _service;

    public RedisCacheServiceTests()
    {
        _cacheMock = new Mock<IDistributedCache>();
        _service = new RedisCacheService(_cacheMock.Object);
    }

    [Fact]
    public async Task TryGetAsync_WhenCacheExists_ReturnsDeserializedLatestResult()
    {
        // Arrange
        var key = "test:latest:usd";
        var expected = new LatestResult(
            Base: "USD",
            Date: new DateTime(2025, 9, 11),
            Rates: new Dictionary<string, double> { { "EUR", 0.85 } }
        );

        var serialized = JsonSerializer.SerializeToUtf8Bytes(expected);
        _cacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serialized);

        // Act
        var result = await _service.TryGetAsync<LatestResult>(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Base, result!.Base);
        Assert.Equal(expected.Rates["EUR"], result.Rates["EUR"]);
    }

    [Fact]
    public async Task TryGetAsync_WhenCacheDoesNotExist_ReturnsDefault()
    {
        // Arrange
        var key = "test:latest:usd";
        _cacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        // Act
        var result = await _service.TryGetAsync<LatestResult>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_StoresSerializedLatestResultInCache()
    {
        // Arrange
        var baseCurrency = "EUR";
        var key = $"test:latest:{baseCurrency}";
        var obj = new LatestResult(
            Base: baseCurrency,
            Date: new DateTime(2025, 9, 11),
            Rates: new Dictionary<string, double> { { "USD", 1.1 } }
        );
        var expireAt = TimeSpan.FromHours(1);

        // Act
        await _service.SetAsync(key, obj, expireAt);

        // Assert
        _cacheMock.Verify(c => c.SetAsync(
            key,
            It.Is<byte[]>(b => Encoding.UTF8.GetString(b).Contains(baseCurrency)),
            It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == expireAt),
            It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
