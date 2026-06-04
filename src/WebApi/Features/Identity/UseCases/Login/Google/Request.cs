using FluentValidation;
using WebApi.Features.Identity.Shared.Authorization;

namespace WebApi.Features.Identity.UseCases.Login.Google;

public record GoogleLoginRequest(string IdToken, string? RegistrationKind);

public sealed class GoogleLoginRequestValidator : AbstractValidator<GoogleLoginRequest>
{
    private readonly string[] SupportedRegistrationKind =
    [
        IdentityRoles.PROFESIONAL,
        IdentityRoles.PACIENTE,
    ];

    public GoogleLoginRequestValidator()
    {
        RuleFor(x => x.IdToken).NotEmpty().WithMessage("El IdToken es requerido.");

        RuleFor(x => x.RegistrationKind)
            .Must(x => string.IsNullOrWhiteSpace(x) || SupportedRegistrationKind.Contains(x))
            .WithMessage(
                $"El tipo de registro debe ser uno de los siguientes: {string.Join(", ", SupportedRegistrationKind)}."
            );
    }
}
