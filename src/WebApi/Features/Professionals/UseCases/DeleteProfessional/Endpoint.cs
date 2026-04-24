using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.DeleteProfessional;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:int}", Handle).AllowAnonymous().WithName("Professionals_Delete");
    }

    private static async Task<IResult> Handle(
        int id,
        DeleteProfessionalHandler handler,
        CancellationToken ct
    )
    {
        var result = await handler.HandleAsync(id, ct);
        return result.ToHttpResult();
    }
}
