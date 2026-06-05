using WebApi.Features.Identity.Domain;

namespace WebApi.Features.People.Domain;

public class Person
{
    public int Id { get; set; }
    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Dni { get; set; }
    public string? PhoneNumber { get; set; }
    public string Email { get; set; } = string.Empty;
}
