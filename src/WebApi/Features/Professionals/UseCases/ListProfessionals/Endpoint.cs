using Microsoft.AspNetCore.Mvc;
using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.ListProfessionals;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", Handle).AllowAnonymous().WithName("Professionals_List");
    }

    private static async Task<IResult> Handle(
        [AsParameters] ListProfessionalsRequest request,
        ListProfessionalsHandler handler,
        CancellationToken ct
    )
    {
        var result = await handler.HandleAsync(request, ct);
        return result.ToHttpResult();
    }
}
