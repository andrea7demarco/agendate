namespace WebApi.Features.Identity.UseCases.Register;

public record RegisterResponse(string UserId, string Email, string? FirstName, string? LastName);
