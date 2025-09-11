using Application.Constants;
using Application.Interfaces;
using Infrastructure.Currency.DependencyInjection;
using Infrastructure.Currency.Providers;
using Infrastructure.Currency.Services;
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
        services.AddAuthorizationBuilder()
            .AddPolicy(PolicyConstants.RequireUser, policy => policy.RequireRole(RoleConstants.User))
            .AddPolicy(PolicyConstants.RequireAdmin, policy => policy.RequireRole(RoleConstants.Admin));

        #endregion

        #region Services

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICurrencyRateProvider, CurrencyService>();
        services.AddScoped<FrankfurterCurrencyProvider>();
        services.AddScoped<ICurrencyRateProviderFactory, CurrencyRateProviderFactory>();

        #endregion
    }
}
