using Application.Common.Interfaces;
using Infrastructure.Security.Configurations;
using Infrastructure.Security.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.Security;

public static class DependencyInjection
{
    public static void RegisterSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        var tokenConfiguration = configuration.GetRequiredSection(nameof(TokenConfiguration)).Get<TokenConfiguration>()!;
        services.AddSingleton(tokenConfiguration);
        services.AddSingleton<ITokenProvider, TokenProvider>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfiguration.SecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = tokenConfiguration.Issuer,
                    ValidateAudience = true,
                    ValidAudience = tokenConfiguration.Audience
                };
            });

        services.AddAuthorization();
    }
}
