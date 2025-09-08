using Application.Interfaces;
using Infrastructure.Security.Configurations;
using Infrastructure.Security.Providers;
using Infrastructure.Security.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        #region Authentication and Authorization

        services.Configure<TokenConfiguration>(configuration.GetSection("TokenConfiguration"));
        services.AddSingleton<ITokenProvider, TokenProvider>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, o =>
        {
            var tokenConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<TokenConfiguration>>().Value;

            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfiguration.SecretKey)),
                ValidateIssuer = true,
                ValidIssuer = tokenConfiguration.Issuer,
                ValidateAudience = true,
                ValidAudience = tokenConfiguration.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        #endregion

        #region Services

        services.AddScoped<IAuthService, AuthService>();

        #endregion
    }
}
