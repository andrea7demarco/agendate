using WebApi.Shared.Results;

namespace WebApi.Features.Identity.UseCases.Refresh;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/refresh", Handle).RequireAuthorization().WithName("Identity_Refresh");
    }

    private static async Task<IResult> Handle(RefreshTokenHandler handler, CancellationToken ct)
    {
        var result = await handler.HandleAsync(ct);
        return result.ToHttpResult();
    }
}
