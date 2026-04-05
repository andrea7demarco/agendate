using FluentValidation;
using WebApi.Features.Identity.Shared.Authorization;

namespace WebApi.Features.Identity.UseCases.Register;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string RegistrationKind
);

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private readonly string[] SupportedRegistrationKind =
    [
        IdentityRoles.PROFESIONAL,
        IdentityRoles.PACIENTE,
    ];

    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();

        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);

        RuleFor(x => x.FirstName).MaximumLength(100);

        RuleFor(x => x.LastName).MaximumLength(100);

        RuleFor(x => x.RegistrationKind)
            .NotEmpty()
            .WithMessage("El tipo de registro es requerido.")
            .Must(x => SupportedRegistrationKind.Contains(x))
            .WithMessage(
                $"El tipo de registro debe ser uno de los siguientes: {string.Join(", ", SupportedRegistrationKind)}."
            );
    }
}
