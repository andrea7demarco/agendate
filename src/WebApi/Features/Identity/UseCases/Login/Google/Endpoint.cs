using System.ComponentModel.DataAnnotations;
using FluentValidation;
using WebApi.Features.Identity.UseCases.Login.Local;
using WebApi.Shared.Results;

namespace WebApi.Features.Identity.UseCases.Login.Google;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/login/google", Handle).AllowAnonymous().WithName("Identity_GoogleLogin");
    }

    private static async Task<IResult> Handle(
        GoogleLoginRequest request,
        GoogleLoginHandler handler,
        IValidator<GoogleLoginRequest> validator,
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
