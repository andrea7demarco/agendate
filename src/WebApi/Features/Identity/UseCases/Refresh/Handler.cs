using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApi.Features.Identity.Domain;
using WebApi.Features.Identity.Shared.Auth;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;

namespace WebApi.Features.Identity.UseCases.Refresh;

public class RefreshTokenHandler(
    HttpContext httpContext,
    IApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    ITokenProvider tokenProvider,
    IOptions<JwtOptions> jwtOptionsAccessor
)
{
    public async Task<Result<RefreshTokenResponse>> HandleAsync(CancellationToken ct)
    {
        if (
            !httpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken)
            || string.IsNullOrWhiteSpace(refreshToken)
        )
            return Result<RefreshTokenResponse>.Failure(IdentityErrors.RefreshTokenNotFound);

        var refreshTokenHash = tokenProvider.HashToken(refreshToken);
        var currentToken = await dbContext
            .RefreshTokens.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == refreshTokenHash, ct);

        if (currentToken is null || currentToken.IsActive)
            return Result<RefreshTokenResponse>.Failure(IdentityErrors.RefreshTokenNotFound);

        var user = currentToken.User;
        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;

        var jwtOpts = jwtOptionsAccessor.Value;
        var newAccessToken = tokenProvider.CreateAccessToken(user, role);
        var newRefreshToken = tokenProvider.CreateRefreshToken();
        var newRefreshTokenHash = tokenProvider.HashToken(newRefreshToken);

        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = newRefreshTokenHash,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(jwtOpts.RefreshTokenDays),
        };
        dbContext.RefreshTokens.Add(newRefreshTokenEntity);

        currentToken.RevokedAtUtc = DateTime.UtcNow;
        currentToken.ReplacedByTokenId = newRefreshTokenEntity.Id;
        await dbContext.SaveChangesAsync(ct);

        httpContext.Response.AppendRefreshTokenCookie(newRefreshToken, currentToken.ExpiresAtUtc);

        return Result<RefreshTokenResponse>.Success(
            new RefreshTokenResponse(newAccessToken, jwtOpts.AccessTokenMinutes * 60)
        );
    }
}
