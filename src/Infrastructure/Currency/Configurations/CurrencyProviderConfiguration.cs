namespace Infrastructure.Currency.Configurations;

public sealed class CurrencyProviderConfiguration
{
    public string Url { get; set; } = string.Empty;
    public HealthCheckSettings HealthCheck { get; set; } = new();
}

public sealed class HealthCheckSettings
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}

