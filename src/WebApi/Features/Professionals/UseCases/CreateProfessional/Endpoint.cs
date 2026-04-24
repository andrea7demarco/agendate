using FluentValidation;
using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.CreateProfessional;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", Handle).AllowAnonymous().WithName("Professionals_Create");
    }

    private static async Task<IResult> Handle(
        CreateProfessionalRequest request,
        CreateProfessionalHandler handler,
        IValidator<CreateProfessionalRequest> validator,
        CancellationToken ct
    )
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return validationResult.ToResult().ToHttpResult();

        var result = await handler.HandleAsync(request, ct);
        return result.ToHttpResult();
    }
}
