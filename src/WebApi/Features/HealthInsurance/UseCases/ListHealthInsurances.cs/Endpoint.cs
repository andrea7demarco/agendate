using WebApi.Shared.Results;

namespace WebApi.Features.HealthInsurances.UseCases.ListHealthInsurances;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", Handle).AllowAnonymous().WithName("HealthInsurances_List");
    }

    private static async Task<IResult> Handle(
        ListHealthInsurancesHandler handler,
        CancellationToken ct
    )
    {
        var result = await handler.HandleAsync(ct);
        return result.ToHttpResult();
    }
}
