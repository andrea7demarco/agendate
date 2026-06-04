using System.Text.Json;
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
        HttpRequest httpRequest,
        CreateProfessionalHandler handler,
        IValidator<CreateProfessionalRequest> validator,
        CancellationToken ct
    )
    {
        CreateProfessionalRequest? request;

        try
        {
            request = await httpRequest.ReadFromJsonAsync<CreateProfessionalRequest>(
                cancellationToken: ct
            );
        }
        catch (JsonException ex)
        {
            return Result<CreateProfessionalResponse>
                .Failure(Error.BadRequest($"JSON inválido: {ex.Message}"))
                .ToHttpResult();
        }
        catch (InvalidOperationException ex)
        {
            return Result<CreateProfessionalResponse>
                .Failure(Error.BadRequest($"Request inválido: {ex.Message}"))
                .ToHttpResult();
        }

        if (request is null)
        {
            return Result<CreateProfessionalResponse>
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
