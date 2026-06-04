using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.ListSpecialties;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/specialties", Handle)
            .AllowAnonymous()
            .WithName("Professionals_ListSpecialties");
    }

    private static async Task<IResult> Handle(ListSpecialtiesHandler handler, CancellationToken ct)
    {
        var result = await handler.HandleAsync(ct);
        return result.ToHttpResult();
    }
}
