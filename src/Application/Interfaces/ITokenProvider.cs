using Application.Dtos.Auth;

namespace Application.Interfaces
{
    public interface ITokenProvider
    {
        Task<TokenResult> GenerateTokenAsync(TokenRequest tokenRequest, CancellationToken cancellationToken);
    }
}
