using FluentValidation;
using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.UpdateProfessional;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:int}", Handle).AllowAnonymous().WithName("Professionals_Update");
    }

    private static async Task<IResult> Handle(
        int id,
        UpdateProfessionalRequest request,
        UpdateProfessionalHandler handler,
        IValidator<UpdateProfessionalRequest> validator,
        CancellationToken ct
    )
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return validationResult.ToResult().ToHttpResult();

        var result = await handler.HandleAsync(id, request, ct);
        return result.ToHttpResult();
    }
}
