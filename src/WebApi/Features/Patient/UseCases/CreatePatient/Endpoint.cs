using System.Text.Json;
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
        HttpRequest httpRequest,
        CreatePatientHandler handler,
        IValidator<CreatePatientRequest> validator,
        CancellationToken ct
    )
    {
        CreatePatientRequest? request;

        try
        {
            request = await httpRequest.ReadFromJsonAsync<CreatePatientRequest>(
                cancellationToken: ct
            );
        }
        catch (JsonException ex)
        {
            return Result<CreatePatientResponse>
                .Failure(Error.BadRequest($"JSON inválido: {ex.Message}"))
                .ToHttpResult();
        }
        catch (InvalidOperationException ex)
        {
            return Result<CreatePatientResponse>
                .Failure(Error.BadRequest($"Request inválido: {ex.Message}"))
                .ToHttpResult();
        }

        if (request is null)
        {
            return Result<CreatePatientResponse>
                .Failure(Error.BadRequest("El body del request es obligatorio."))
                .ToHttpResult();
        }

        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return validationResult.ToResult().ToHttpResult();

        var result = await handler.HandleAsync(request, ct);
        return result.ToHttpResult();
    }
}
