namespace WebApi.Features.Identity.UseCases.Login;

public sealed record LoginResponse(string AccessToken, AuthenticatedUserDto User);

public sealed record AuthenticatedUserDto(
    string UserId,
    string Email,
    string? FirstName,
    string? LastName,
    string Role,
    bool RegistrationCompleted
);
