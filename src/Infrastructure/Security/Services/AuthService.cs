using Application.Dtos.Auth;
using Application.Interfaces;
using System.Collections.Concurrent;

namespace Infrastructure.Security.Services;

public sealed class AuthService : IAuthService
{
    private static readonly ConcurrentDictionary<string, (Guid Id, string Password, string[] Roles)> _users = new();

    public AuthService()
    {
        _users["admin@email.com"] = (Guid.NewGuid(), "123456", new[] { "Admin", "User" });
        _users["user@email.com"] = (Guid.NewGuid(), "123456", new[] { "User" });
    }

    public Task<LoginResult> ValidateCredentialsAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
    {
        if (_users.TryGetValue(loginRequest.UserName, out var user) && loginRequest.Password == user.Password)
            return Task.FromResult(new LoginResult(true, user.Id, loginRequest.UserName));

        return Task.FromResult(new LoginResult(false));
    }
}
