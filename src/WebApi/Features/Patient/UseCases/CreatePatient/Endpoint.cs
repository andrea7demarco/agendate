using FluentValidation;
using WebApi.Shared.Results;

namespace WebApi.Features.Patients.UseCases.CreatePatient;

public static class Endpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", Handle).AllowAnonymous().WithName("Patients_Create");
    }

    private static async Task<IResult> Handle(
        CreatePatientRequest request,
        CreatePatientHandler handler,
        IValidator<CreatePatientRequest> validator,
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
