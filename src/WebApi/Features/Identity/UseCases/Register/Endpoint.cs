using FluentValidation;
using WebApi.Shared.Results;

namespace WebApi.Features.Identity.UseCases.Register;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/register", Handle).AllowAnonymous().WithName("Identity_Register");
    }

    private static async Task<IResult> Handle(
        RegisterRequest request,
        IValidator<RegisterRequest> validator,
        RegisterHandler handler,
        CancellationToken ct
    )
    {
        var validation = await validator.ValidateAsync(request, ct);
        var validationResult = validation.ToResult("auth.invalid_register_request");
        if (validationResult.IsFailure)
            return validationResult.ToHttpResult();

        var result = await handler.HandleAsync(request, ct);
        return result.ToHttpResult();
    }
}
