namespace WebApi.Features.Identity.UseCases.Refresh;

public record RefreshTokenResponse(string AccessToken, int ExpiresInSeconds);
