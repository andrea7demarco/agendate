using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.Identity.Domain;
using WebApi.Features.Identity.Shared.Auth;
using WebApi.Shared.Persistence;

namespace WebApi.Features.Identity.UseCases.Logout;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/logout", Handle).RequireAuthorization().WithName("Identity_Logout");
    }

    private static async Task<IResult> Handle(
        HttpContext httpContext,
        SignInManager<ApplicationUser> signInManager,
        IApplicationDbContext dbContext,
        ITokenProvider tokenProvider,
        CancellationToken ct
    )
    {
        if (
            httpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken)
            && !string.IsNullOrWhiteSpace(refreshToken)
        )
        {
            var refreshTokenHash = tokenProvider.HashToken(refreshToken);
            var tokenEntity = await dbContext.RefreshTokens.FirstOrDefaultAsync(
                x => x.TokenHash == refreshTokenHash,
                ct
            );

            if (tokenEntity is not null && tokenEntity.RevokedAtUtc is null)
            {
                tokenEntity.RevokedAtUtc = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(ct);
            }
        }

        httpContext.Response.DeleteRefreshTokenCookie();

        await signInManager.SignOutAsync();
        return TypedResults.NoContent();
    }
}
