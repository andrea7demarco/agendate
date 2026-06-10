using WebApi.Shared.Results;

namespace WebApi.Features.Patients.UseCases.DeletePatient;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:int}", Handle).AllowAnonymous().WithName("Patients_Delete");
    }

    private static async Task<IResult> Handle(
        int id,
        DeletePatientHandler handler,
        CancellationToken ct
    )
    {
        var result = await handler.HandleAsync(id, ct);
        return result.ToHttpResult();
    }
}
