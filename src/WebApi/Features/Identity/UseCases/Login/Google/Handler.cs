using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using WebApi.Features.Identity.Domain;
using WebApi.Features.Identity.Shared.Auth;
using WebApi.Shared.Persistence;
using WebApi.Shared.Providers;
using WebApi.Shared.Results;

namespace WebApi.Features.Identity.UseCases.Login.Google;

public sealed class GoogleLoginHandler(
    UserManager<ApplicationUser> userManager,
    ITokenProvider tokenProvider,
    IOptions<JwtOptions> jwtOptionsAccessor,
    IOptions<GoogleAuthOptions> googleAuthOptionsAccessor,
    IApplicationDbContext dbContext,
    IHttpContextProvider httpContextProvider
)
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly HttpContext httpContext =
        httpContextProvider.GetCurrent()
        ?? throw new InvalidOperationException("No HttpContext available.");

    public async Task<Result<LoginResponse>> HandleAsync(
        GoogleLoginRequest request,
        CancellationToken ct
    )
    {
        GoogleJsonWebSignature.Payload payload;

        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [googleAuthOptionsAccessor.Value.ClientId],
            };

            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
        }
        catch
        {
            return Result<LoginResponse>.Failure(IdentityErrors.ExternalLoginInfoNotFound);
        }

        if (string.IsNullOrWhiteSpace(payload.Subject) || string.IsNullOrWhiteSpace(payload.Email))
            return Result<LoginResponse>.Failure(IdentityErrors.ExternalEmailNotFound);

        if (payload.EmailVerified != true)
            return Result<LoginResponse>.Failure(IdentityErrors.ExternalEmailNotFound);

        var loginProvider = AuthProviders.GOOGLE;
        var providerKey = payload.Subject;

        var user = await _userManager.FindByLoginAsync(loginProvider, providerKey);
        if (user is null)
        {
            user = await _userManager.FindByEmailAsync(payload.Email);

            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    EmailConfirmed = payload.EmailVerified,
                    RegistrationCompleted = false,
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return Result<LoginResponse>.Failure(
                        Error.Validation(
                            "auth.user_creation_failed",
                            "No se pudo crear el usuario.",
                            createResult
                                .Errors.GroupBy(x => x.Code)
                                .ToDictionary(
                                    g => g.Key,
                                    g => g.Select(x => x.Description).ToArray()
                                )
                        )
                    );

                if (!string.IsNullOrWhiteSpace(request.RegistrationKind))
                {
                    var addRoleResult = await _userManager.AddToRoleAsync(
                        user,
                        request.RegistrationKind
                    );

                    if (!addRoleResult.Succeeded)
                    {
                        return Result<LoginResponse>.Failure(
                            Error.Validation(
                                "auth.user_role_assignment_failed",
                                "No se pudo asignar el rol base al usuario.",
                                addRoleResult
                                    .Errors.GroupBy(x => x.Code)
                                    .ToDictionary(
                                        g => g.Key,
                                        g => g.Select(x => x.Description).ToArray()
                                    )
                            )
                        );
                    }
                }
            }

            var addLoginResult = await _userManager.AddLoginAsync(
                user,
                new UserLoginInfo(loginProvider, providerKey, loginProvider)
            );

            if (!addLoginResult.Succeeded)
                return Result<LoginResponse>.Failure(
                    Error.Validation(
                        "auth.external_login_link_failed",
                        "No se pudo vincular el login externo al usuario.",
                        addLoginResult
                            .Errors.GroupBy(x => x.Code)
                            .ToDictionary(g => g.Key, g => g.Select(x => x.Description).ToArray())
                    )
                );
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;
        var jwt = jwtOptionsAccessor.Value;

        var accessToken = tokenProvider.CreateAccessToken(user, role);
        var refreshToken = tokenProvider.CreateRefreshToken();
        var refreshTokenHash = tokenProvider.HashToken(refreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(jwt.RefreshTokenDays),
        };

        dbContext.RefreshTokens.Add(refreshTokenEntity);
        await dbContext.SaveChangesAsync(ct);

        httpContext.Response.AppendRefreshTokenCookie(
            refreshToken,
            refreshTokenEntity.ExpiresAtUtc
        );

        return Result<LoginResponse>.Success(
            new LoginResponse(
                AccessToken: accessToken,
                User: new AuthenticatedUserDto(
                    UserId: user.Id,
                    Email: user.Email ?? string.Empty,
                    FirstName: user.FirstName,
                    LastName: user.LastName,
                    Role: role,
                    RegistrationCompleted: user.RegistrationCompleted
                )
            )
        );
    }
}
