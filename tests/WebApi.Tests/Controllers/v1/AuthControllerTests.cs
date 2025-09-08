using Application.Dtos.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers.Auth;

namespace WebApi.Tests.Controllers.v1;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock = new();
    private readonly Mock<ITokenProvider> _tokenProviderMock = new();

    private AuthController CreateController()
    {
        return new AuthController(_authServiceMock.Object, _tokenProviderMock.Object);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var controller = CreateController();
        var loginRequest = new LoginRequest("any.user@email.com", "WrongPassword");

        _authServiceMock.Setup(setup => setup.ValidateCredentialsAsync(loginRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResult(IsAuthenticated: false, UserId: Guid.Empty, Username: string.Empty));

        // Act
        var result = await controller.Login(loginRequest, CancellationToken.None);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var controller = CreateController();
        var username = "admin@email.com";
        var loginRequest = new LoginRequest(username, "123456");

        _authServiceMock.Setup(setup => setup.ValidateCredentialsAsync(loginRequest, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LoginResult(IsAuthenticated: true, UserId: Guid.NewGuid(), Username: username));

        var expectedToken = "randomToken";
        var expectedExpiresAtUtc = DateTimeOffset.UtcNow;
        var expectedResult = new TokenResult(
            AccessToken: expectedToken,
            TokenType: JwtBearerDefaults.AuthenticationScheme,
            ExpiresAtUtc: expectedExpiresAtUtc
        );

        _tokenProviderMock
            .Setup(t => t.GenerateTokenAsync(It.IsAny<TokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await controller.Login(loginRequest, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var tokenResult = Assert.IsType<TokenResult>(okResult.Value);
        Assert.Equal(expectedResult.AccessToken, tokenResult.AccessToken);
        Assert.Equal(expectedResult.TokenType, tokenResult.TokenType);
        Assert.Equal(expectedResult.ExpiresAtUtc, tokenResult.ExpiresAtUtc);
    }
}
