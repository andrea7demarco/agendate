using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.ListProfessionals;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", Handle).AllowAnonymous().WithName("Professionals_List");
    }

    private static async Task<IResult> Handle(
        ListProfessionalsHandler handler,
        CancellationToken ct
    )
    {
        var result = await handler.HandleAsync(ct);
        return result.ToHttpResult();
    }
}
