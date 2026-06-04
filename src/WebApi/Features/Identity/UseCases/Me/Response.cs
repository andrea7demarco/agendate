namespace WebApi.Features.Identity.UseCases.Me;

public sealed record MeResponse(
    string UserId,
    string? Email,
    string? FirstName,
    string? LastName,
    string Role,
    bool RegistrationCompleted
);
