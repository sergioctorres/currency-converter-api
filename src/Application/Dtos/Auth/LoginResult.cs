namespace Application.Dtos.Auth;

public record LoginResult(
    bool IsAuthenticated,
    Guid UserId = default,
    string Username = ""
);

