using FluentValidation;
using WebApi.Shared.Results;

namespace WebApi.Features.Identity.UseCases.Login.Local;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/login", Handle).AllowAnonymous().WithName("Identity_Login");
    }

    private static async Task<IResult> Handle(
        LocalLoginRequest request,
        LocalLoginHandler handler,
        IValidator<LocalLoginRequest> validator,
        CancellationToken ct
    )
    {
        var validation = await validator.ValidateAsync(request, ct);
        var validationResult = validation.ToResult("auth.invalid_login_request");
        if (validationResult.IsFailure)
            return validationResult.ToHttpResult();

        var result = await handler.HandleAsync(request, ct);
        return result.ToHttpResult();
    }
}
