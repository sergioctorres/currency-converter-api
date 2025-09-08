using Application.Dtos.Auth;
using Application.Interfaces;
using Infrastructure.Security.Configurations;
using Infrastructure.Security.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Infrastructure.Tests.Security.Providers;

public class TokenProviderTests
{
    private readonly ITokenProvider _tokenProvider;

    private readonly TokenConfiguration _config = new()
    {
        SecretKey = "SecretKeyForTest1234567890!1234567890!",
        Issuer = "Issuer",
        Audience = "Audience",
        ExpirationInMinutes = 45
    };

    public TokenProviderTests()
    {
        var options = Options.Create(_config);

        _tokenProvider = new TokenProvider(options);
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldReturn_ValidToken()
    {
        // Arrange
        var request = new TokenRequest("sub", "name");
        var expectedExpiresAtUtc = DateTime.UtcNow.AddMinutes(_config.ExpirationInMinutes);

        // Act
        var result = await _tokenProvider.GenerateTokenAsync(request, CancellationToken.None);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.True(result.ExpiresAtUtc >= expectedExpiresAtUtc);
        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, result.TokenType);
    }
}
