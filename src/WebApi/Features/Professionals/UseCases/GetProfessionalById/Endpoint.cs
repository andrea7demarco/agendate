using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.GetProfessionalById;

public static class Endpoint
{
    //map es un metodo de extension que registra la ruta y de llama desde DependencyInjection.Map....
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:int}", Handle)
            .AllowAnonymous() //permite q cualq acceda
            .WithName("Professionals_GetById");
    }

    private static async Task<IResult> Handle(
        int id,
        GetProfessionalByIdHandler handler,
        CancellationToken ct
    )
    {
        var result = await handler.HandleAsync(id, ct);
        return result.ToHttpResult();
    }
}
