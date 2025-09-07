using Application.Dtos.Auth;
using Application.Interfaces;
using Infrastructure.Security.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Security.Providers;

public sealed class TokenProvider(TokenConfiguration tokenConfiguration) : ITokenProvider
{
    public Task<TokenResult> GenerateTokenAsync(TokenRequest tokenRequest, CancellationToken cancellationToken)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(tokenConfiguration.ExpirationInMinutes);
            
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfiguration.SecretKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, tokenRequest.Subject),
            new(ClaimTypes.Name, tokenRequest.Name)
        };

        var securityToken = new JwtSecurityToken(
            issuer: tokenConfiguration.Issuer,
            audience: tokenConfiguration.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: signingCredentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return Task.FromResult(new TokenResult(accessToken, JwtBearerDefaults.AuthenticationScheme, expiresAtUtc));
    }
}
