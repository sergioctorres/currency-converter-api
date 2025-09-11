using Infrastructure.Currency.Constants;
using Infrastructure.Currency.DependencyInjection;
using Infrastructure.Currency.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tests.Currency.DependencyInjection;

public class CurrencyRateProviderFactoryTests
{
    [Fact]
    public void Create_WithFrankfurterApi_ReturnsFrankfurterCurrencyProvider()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddHttpClient()
            .AddScoped<FrankfurterCurrencyProvider>()
            .BuildServiceProvider();

        var factory = new CurrencyRateProviderFactory(services);

        // Act
        var result = factory.Create(CurrencyRateConstants.Providers.FrankfurterApi);

        // Assert
        Assert.IsType<FrankfurterCurrencyProvider>(result);
    }

    [Fact]
    public void Create_WithUnsupportedProvider_ThrowsNotSupportedException()
    {
        // Arrange
        var services = new ServiceCollection().BuildServiceProvider();
        var factory = new CurrencyRateProviderFactory(services);

        // Act / Assert
        Assert.Throws<NotSupportedException>(() => factory.Create("UnknownProvider"));
    }
}
