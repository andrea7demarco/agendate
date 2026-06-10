using System.Security.Claims;
using WebApi.Shared.Results;

namespace WebApi.Features.Profiles.UseCases.GetMyProfile;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/me", Handle).RequireAuthorization().WithName("Profile_GetMe");
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal user,
        GetMyProfileHandler handler,
        CancellationToken ct
    )
    {
        var applicationUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(applicationUserId))
            return TypedResults.Unauthorized();

        var result = await handler.HandleAsync(applicationUserId, ct);

        return result.ToHttpResult();
    }
}
