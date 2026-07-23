using FluentValidation;
using WebApi.Features.Identity.Shared.Authorization;

namespace WebApi.Features.Identity.UseCases.CompleteRegistration;

public sealed record CompleteRegistrationRequest(
    string RegistrationKind,
    DateOnly? BirthDate,
    int? Gender,
    bool? HasCud,
    string? Dni,
    string? PhoneNumber,
    decimal? ConsultationCost,
    string? AppointmentType,
    string? Address,
    string? City,
    string? Province,
    string? NationalLicense,
    string? ProvincialLicense,
    string? Biography,
    List<int>? SpecialtyIds,
    List<int>? HealthInsuranceIds,
    List<CompleteRegistrationHealthInsuranceRequest>? HealthInsurances
);

public sealed record CompleteRegistrationHealthInsuranceRequest(
    int HealthInsuranceId,
    string? AffiliateNumber,
    string? PlanName
);

public sealed class CompleteRegistrationRequestValidator
    : AbstractValidator<CompleteRegistrationRequest>
{
    public CompleteRegistrationRequestValidator()
    {
        RuleFor(x => x.RegistrationKind)
            .NotEmpty()
            .Must(x => x is IdentityRoles.PACIENTE or IdentityRoles.PROFESIONAL)
            .WithMessage("El tipo de registro debe ser paciente o profesional.");

        When(
            x => x.RegistrationKind == IdentityRoles.PACIENTE,
            () =>
            {
                RuleFor(x => x.BirthDate)
                    .NotNull()
                    .WithMessage("La fecha de nacimiento es obligatoria.");
                RuleFor(x => x.Gender)
                    .NotNull()
                    .InclusiveBetween(1, 3)
                    .WithMessage("El genero debe ser Masculino, Femenino u Otro.");
            }
        );

        When(
            x => x.RegistrationKind == IdentityRoles.PROFESIONAL,
            () =>
            {
                RuleFor(x => x.Dni).NotEmpty().WithMessage("El DNI es obligatorio.");
                RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("El telefono es obligatorio.");
                RuleFor(x => x.ConsultationCost)
                    .NotNull()
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("El costo de consulta es obligatorio.");
                RuleFor(x => x.AppointmentType)
                    .NotEmpty()
                    .Must(x => x is "Presencial" or "Online" or "Ambos")
                    .WithMessage("AppointmentType debe ser 'Presencial', 'Online' o 'Ambos'.");
                RuleFor(x => x.Province).NotEmpty().WithMessage("La provincia es obligatoria.");
                RuleFor(x => x.NationalLicense)
                    .NotEmpty()
                    .WithMessage("La matricula nacional es obligatoria.");
                RuleFor(x => x.ProvincialLicense)
                    .NotEmpty()
                    .Matches(@"^\d+$")
                    .WithMessage("La matricula provincial debe ser numerica.");
                RuleFor(x => x.SpecialtyIds)
                    .NotNull()
                    .Must(ids => ids is { Count: > 0 })
                    .WithMessage("Debes enviar al menos una especialidad.");
                RuleFor(x => x.City)
                    .NotEmpty()
                    .MaximumLength(100)
                    .When(x => x.RegistrationKind == IdentityRoles.PROFESIONAL);
            }
        );
    }
}
