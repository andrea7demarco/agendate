using Microsoft.AspNetCore.Identity;

namespace WebApi.Features.Identity.Domain;

public sealed class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool RegistrationCompleted { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
