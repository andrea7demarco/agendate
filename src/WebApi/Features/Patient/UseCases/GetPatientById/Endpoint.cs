using WebApi.Shared.Results;

namespace WebApi.Features.Patients.UseCases.GetPatientById;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:int}", Handle)
            .AllowAnonymous() //permite q cualq acceda
            .WithName("Patients_GetById");
    }

    private static async Task<IResult> Handle(
        int id,
        GetPatientByIdHandler handler,
        CancellationToken ct
    )
    {
        var result = await handler.HandleAsync(id, ct);
        return result.ToHttpResult();
    }
}
