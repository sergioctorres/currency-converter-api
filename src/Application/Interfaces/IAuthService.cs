using Application.Dtos.Auth;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<LoginResult> ValidateCredentialsAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default);
}
