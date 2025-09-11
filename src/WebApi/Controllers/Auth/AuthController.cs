using Application.Dtos.Auth;
using Application.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Auth;

[Route("[controller]")]
[ApiVersionNeutral]
public class AuthController(IAuthService authService, ITokenProvider tokenProvider) : ApiControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var loginResult = await authService.ValidateCredentialsAsync(request, cancellationToken);

        if (loginResult.IsAuthenticated is false) return Unauthorized();

        var tokenRequest = new TokenRequest(loginResult.UserId.ToString(), loginResult.Username);

        return Ok(await tokenProvider.GenerateTokenAsync(tokenRequest, cancellationToken));
    }
}
