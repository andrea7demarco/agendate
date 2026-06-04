using System.Security.Claims;
using FluentValidation;
using WebApi.Shared.Results;

namespace WebApi.Features.Identity.UseCases.CompleteRegistration;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/complete-registration", Handle)
            .RequireAuthorization()
            .WithName("Identity_CompleteRegistration");
    }

    private static async Task<IResult> Handle(
        ClaimsPrincipal claimsPrincipal,
        CompleteRegistrationRequest request,
        CompleteRegistrationHandler handler,
        IValidator<CompleteRegistrationRequest> validator,
        CancellationToken ct
    )
    {
        var validation = await validator.ValidateAsync(request, ct);
        var validationResult = validation.ToResult("auth.invalid_complete_registration_request");
        if (validationResult.IsFailure)
            return validationResult.ToHttpResult();

        var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result
                .Failure(
                    Error.Unauthorized("auth.user_not_authenticated", "Usuario no autenticado.")
                )
                .ToHttpResult();
        }

        var result = await handler.HandleAsync(userId, request, ct);
        return result.ToHttpResult();
    }
}
