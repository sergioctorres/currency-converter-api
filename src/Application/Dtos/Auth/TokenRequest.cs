namespace Application.Dtos.Auth;

public record TokenRequest(
    string Subject,
    string Name,
    string[]? Roles
);

