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
    string? DegreeTitle,
    string? University,
    int? GraduationYear,
    List<int>? SpecialtyIds,
    List<int>? HealthInsuranceIds,
    List<CompleteRegistrationHealthInsuranceRequest>? HealthInsurances,
    List<CompleteRegistrationProfessionalLocationRequest>? Locations,
    List<CompleteRegistrationProfessionalAvailabilityRequest>? Availabilities,
    List<int>? PatientGroups,
    List<CompleteRegistrationProfessionalTrainingRequest>? Trainings
);

public sealed record CompleteRegistrationHealthInsuranceRequest(
    int HealthInsuranceId,
    string? AffiliateNumber,
    string? PlanName
);

public sealed record CompleteRegistrationProfessionalLocationRequest(
    string Name,
    string? FormattedAddress,
    string? Street,
    string? StreetNumber,
    string? City,
    string? Province,
    string? PostalCode,
    decimal? Latitude,
    decimal? Longitude,
    string? ExternalPlaceId,
    string? ExternalProvider,
    string? Instructions
);

public sealed record CompleteRegistrationProfessionalAvailabilityRequest(
    int DayOfWeek,
    int TimeSlot
);

public sealed record CompleteRegistrationProfessionalTrainingRequest(
    string Title,
    string? Institution,
    int? Year,
    string? Description
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
                    .WithMessage("La matrícula nacional es obligatoria.");
                RuleFor(x => x.NationalLicense)
                    .Matches(@"^\d+$")
                    .WithMessage("La matrícula nacional debe ser numérica.");
                RuleFor(x => x.ProvincialLicense)
                    .NotEmpty()
                    .WithMessage("La matrícula provincial es obligatoria.");
                RuleFor(x => x.ProvincialLicense)
                    .Matches(@"^\d+$")
                    .WithMessage("La matrícula provincial debe ser numérica.");
                RuleFor(x => x.SpecialtyIds)
                    .NotNull()
                    .Must(ids => ids is { Count: > 0 })
                    .WithMessage("Debes enviar al menos una especialidad.");
                RuleFor(x => x.City)
                    .NotEmpty()
                    .MaximumLength(100)
                    .When(x => x.RegistrationKind == IdentityRoles.PROFESIONAL);
                RuleFor(x => x.DegreeTitle).MaximumLength(150);
                RuleFor(x => x.University).MaximumLength(150);
                RuleFor(x => x.GraduationYear)
                    .InclusiveBetween(1900, DateTime.Today.Year)
                    .When(x => x.GraduationYear.HasValue)
                    .WithMessage("El año de recibido no es válido.");
                RuleForEach(x => x.Locations)
                    .ChildRules(location =>
                    {
                        location.RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
                        location.RuleFor(x => x.FormattedAddress).NotEmpty().MaximumLength(250);
                        location.RuleFor(x => x.Latitude).NotNull();
                        location.RuleFor(x => x.Longitude).NotNull();
                    });
                RuleForEach(x => x.Availabilities)
                    .ChildRules(availability =>
                    {
                        availability
                            .RuleFor(x => x.DayOfWeek)
                            .InclusiveBetween(0, 6)
                            .WithMessage("El día de disponibilidad no es válido.");
                        availability
                            .RuleFor(x => x.TimeSlot)
                            .InclusiveBetween(1, 4)
                            .WithMessage("La franja horaria no es válida.");
                    });
                RuleFor(x => x.PatientGroups)
                    .Must(groups => groups is null || groups.All(group => group >= 1 && group <= 5))
                    .WithMessage("Los tipos de pacientes seleccionados son inválidos.");
                RuleForEach(x => x.Trainings)
                    .ChildRules(training =>
                    {
                        training.RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
                        training.RuleFor(x => x.Institution).MaximumLength(200);
                        training
                            .RuleFor(x => x.Year)
                            .InclusiveBetween(1900, DateTime.Today.Year)
                            .When(x => x.Year.HasValue)
                            .WithMessage("El año del curso no es válido.");
                        training.RuleFor(x => x.Description).MaximumLength(500);
                    });
            }
        );
    }
}
