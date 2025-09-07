using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Auth;

public record LoginRequest(
    [Required, EmailAddress] string UserName,
    [Required, MinLength(6)] string Password
);