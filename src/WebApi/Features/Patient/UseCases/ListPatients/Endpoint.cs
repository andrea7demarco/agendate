using WebApi.Shared.Results;

namespace WebApi.Features.Patients.UseCases.ListPatients;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", Handle).AllowAnonymous().WithName("Patients_List");
    }

    private static async Task<IResult> Handle(ListPatientsHandler handler, CancellationToken ct)
    {
        var result = await handler.HandleAsync(ct);
        return result.ToHttpResult();
    }
}
