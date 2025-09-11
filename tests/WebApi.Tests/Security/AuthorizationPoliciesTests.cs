using Application.Constants;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WebApi.Tests.Security;

public class AuthorizationPoliciesTests
{
    [Fact]
    public void RequireUserPolicy_ShouldRequireUserRole()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddAuthorizationBuilder()
            .AddPolicy(PolicyConstants.RequireUser, policy => policy.RequireRole(RoleConstants.User));

        var sp = services.BuildServiceProvider();

        var options = sp.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

        // Act
        var policy = options.GetPolicy(PolicyConstants.RequireUser);

        // Assert
        Assert.NotNull(policy);
        var rolesRequirement = policy!.Requirements.OfType<RolesAuthorizationRequirement>().FirstOrDefault();
        Assert.NotNull(rolesRequirement);
        Assert.Contains(RoleConstants.User, rolesRequirement!.AllowedRoles);
    }

    [Fact]
    public void RequireAdminPolicy_ShouldRequireAdminRole()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddAuthorizationBuilder()
            .AddPolicy(PolicyConstants.RequireAdmin, policy => policy.RequireRole(RoleConstants.Admin));

        var sp = services.BuildServiceProvider();

        var options = sp.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

        // Act
        var policy = options.GetPolicy(PolicyConstants.RequireAdmin);

        // Assert
        Assert.NotNull(policy);
        var rolesRequirement = policy!.Requirements.OfType<RolesAuthorizationRequirement>().FirstOrDefault();
        Assert.NotNull(rolesRequirement);
        Assert.Contains(RoleConstants.Admin, rolesRequirement!.AllowedRoles);
    }
}
