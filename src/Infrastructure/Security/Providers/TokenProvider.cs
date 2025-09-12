using Application.Dtos.Auth;
using Application.Interfaces;
using Infrastructure.Security.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Security.Providers;

public sealed class TokenProvider(IOptions<TokenConfiguration> options) : ITokenProvider
{
    private readonly TokenConfiguration _tokenConfiguration = options.Value;

    public Task<TokenResult> GenerateTokenAsync(TokenRequest tokenRequest, CancellationToken cancellationToken = default)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_tokenConfiguration.ExpirationInMinutes);
            
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfiguration.SecretKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, tokenRequest.Subject),
            new(ClaimTypes.Name, tokenRequest.Name)
        };

        if (tokenRequest.Roles?.Any() ?? false)
        {
            foreach (var role in tokenRequest.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var securityToken = new JwtSecurityToken(
            issuer: _tokenConfiguration.Issuer,
            audience: _tokenConfiguration.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: signingCredentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return Task.FromResult(new TokenResult(accessToken, JwtBearerDefaults.AuthenticationScheme, expiresAtUtc));
    }
}
