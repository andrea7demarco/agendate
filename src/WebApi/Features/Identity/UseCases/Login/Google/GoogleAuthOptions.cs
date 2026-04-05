namespace WebApi.Features.Identity.UseCases.Login.Google;

public class GoogleAuthOptions
{
    public const string SectionName = "GoogleAuth";
    public string ClientId { get; set; } = null!;
}
