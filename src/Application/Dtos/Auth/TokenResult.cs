namespace Application.Dtos.Auth;

public record TokenResult(
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAtUtc
);

