using Application.Dtos.Auth;
using Application.Interfaces;
using Infrastructure.Security.Services;

namespace Infrastructure.Tests.Security.Services;

public class AuthServiceTests
{
    private readonly IAuthService _authService;

    public AuthServiceTests()
    {
        _authService = new AuthService();
    }

    [Theory]
    [InlineData("admin@email.com", "123456")]
    [InlineData("user@email.com", "123456")]
    public async Task ValidateCredentialsAsync_WithValidUser_ReturnsAuthenticatedResult(string username, string password)
    {
        // Arrange
        var loginRequest = new LoginRequest(username, password);

        // Act
        var result = await _authService.ValidateCredentialsAsync(loginRequest, CancellationToken.None);

        // Assert
        Assert.True(result.IsAuthenticated);
        Assert.Equal(username, result.Username);
        Assert.NotEqual(Guid.Empty, result.UserId);
    }

    [Theory]
    [InlineData("admin@email.com", "wrongPassword")]
    [InlineData("user@email.com", "wrongPassword")]
    public async Task ValidateCredentialsAsync_WithWrongPassword_ReturnsUnauthenticatedResult(string username, string password)
    {
        // Arrange
        var loginRequest = new LoginRequest(username, password);

        // Act
        var result = await _authService.ValidateCredentialsAsync(loginRequest, CancellationToken.None);

        // Assert
        Assert.False(result.IsAuthenticated);
        Assert.Equal(string.Empty, result.Username);
        Assert.Equal(Guid.Empty, result.UserId);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithNonExistent_ReturnsUnauthenticatedResult()
    {
        // Arrange
        var loginRequest = new LoginRequest("non.existent.user@email.com", "anyPassword");

        // Act
        var result = await _authService.ValidateCredentialsAsync(loginRequest, CancellationToken.None);

        // Assert
        Assert.False(result.IsAuthenticated);
        Assert.Equal(string.Empty, result.Username);
        Assert.Equal(Guid.Empty, result.UserId);
    }
}
