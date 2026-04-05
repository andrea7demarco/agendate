using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using WebApi.Features.Identity.Domain;
using WebApi.Features.Identity.Shared.Auth;
using WebApi.Features.Identity.UseCases.Login;
using WebApi.Shared.Persistence;
using WebApi.Shared.Providers;
using WebApi.Shared.Results;

namespace WebApi.Features.Identity.UseCases.Login.Local;

public sealed class LocalLoginHandler(
    IHttpContextProvider httpContextProvider,
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ITokenProvider tokenProvider,
    IOptions<JwtOptions> jwtOptionsAccessor,
    IApplicationDbContext dbContext
)
{
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly HttpContext httpContext =
        httpContextProvider.GetCurrent()
        ?? throw new InvalidOperationException("No HttpContext available.");

    public async Task<Result<LoginResponse>> HandleAsync(
        LocalLoginRequest request,
        CancellationToken ct
    )
    {
        var jwtOpts = jwtOptionsAccessor.Value;

        var user = await _userManager.FindByEmailAsync(request.Email!);
        if (user is null)
            return Result<LoginResponse>.Failure(IdentityErrors.InvalidCredentials);

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password!,
            lockoutOnFailure: true
        );

        if (signInResult.IsLockedOut)
            return Result<LoginResponse>.Failure(IdentityErrors.UserLocked);
        if (!signInResult.Succeeded)
            return Result<LoginResponse>.Failure(IdentityErrors.InvalidCredentials);

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;

        var accesstoken = tokenProvider.CreateAccessToken(user, role);
        var refreshToken = tokenProvider.CreateRefreshToken();
        var hashedRefreshToken = tokenProvider.HashToken(refreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = hashedRefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(jwtOpts.RefreshTokenDays),
        };
        dbContext.RefreshTokens.Add(refreshTokenEntity);
        await dbContext.SaveChangesAsync(ct);

        httpContext.Response.AppendRefreshTokenCookie(
            refreshToken,
            refreshTokenEntity.ExpiresAtUtc
        );

        return Result<LoginResponse>.Success(
            new LoginResponse(
                AccessToken: accesstoken,
                User: new AuthenticatedUserDto(
                    user.Id,
                    user.Email!,
                    user.FirstName,
                    user.LastName,
                    role,
                    user.RegistrationCompleted
                )
            )
        );
    }
}
