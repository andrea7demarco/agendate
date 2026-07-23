using System.Security.Claims;
using FluentValidation;
using WebApi.Shared.Results;

namespace WebApi.Features.Patients.UseCases.UpdatePatient;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:int}", Handle).RequireAuthorization().WithName("Patients_Update");
    }

    private static async Task<IResult> Handle(
        int id,
        ClaimsPrincipal user,
        UpdatePatientRequest request,
        UpdatePatientHandler handler,
        IValidator<UpdatePatientRequest> validator,
        CancellationToken ct
    )
    {
        var applicationUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(applicationUserId))
            return TypedResults.Unauthorized();

        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return validationResult.ToResult().ToHttpResult();

        var result = await handler.HandleAsync(id, applicationUserId, request, ct);
        return result.ToHttpResult();
    }
}
