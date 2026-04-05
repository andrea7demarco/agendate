using System.Security.Claims;

namespace WebApi.Features.Identity.UseCases.Me;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/me", Handle).RequireAuthorization().WithName("Identity_Me");
    }

    private static async Task<IResult> Handle(ClaimsPrincipal user)
    {
        return TypedResults.Ok(
            new MeResponse(
                UserId: user.FindFirstValue(ClaimTypes.NameIdentifier)!,
                Email: user.FindFirstValue(ClaimTypes.Email)!,
                FirstName: user.FindFirstValue(ClaimTypes.GivenName),
                LastName: user.FindFirstValue(ClaimTypes.Surname),
                Role: user.FindFirstValue(ClaimTypes.Role)!
            )
        );
    }
}
