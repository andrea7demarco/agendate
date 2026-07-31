using FluentValidation;

namespace WebApi.Features.Professionals.UseCases.CreateProfessional;

public class CreateProfessionalRequestValidator : AbstractValidator<CreateProfessionalRequest>
{
    public CreateProfessionalRequestValidator()
    {
        RuleFor(x => x.ApplicationUserId).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.ConsultationCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Province).NotEmpty().MaximumLength(80);
        RuleFor(x => x.ProvincialLicense)
            .NotEmpty()
            .Matches(@"^\d+$")
            .WithMessage("La matrícula provincial debe ser numérica.");
        RuleFor(x => x.AppointmentType)
            .Must(BeValidAppointmentType)
            .WithMessage("AppointmentType debe ser 'Presencial', 'Online' o 'Ambos'.");
        RuleFor(x => x.SpecialtyIds)
            .NotNull()
            .Must(ids => ids.Count > 0)
            .WithMessage("Debes enviar al menos una especialidad.");
        RuleFor(x => x.HealthInsuranceIds)
            .Must(ids => ids is null || ids.All(id => id > 0))
            .WithMessage("Las obras sociales seleccionadas son invalidas.");
        RuleFor(x => x.DegreeTitle).MaximumLength(150);
        RuleFor(x => x.University).MaximumLength(150);
        RuleFor(x => x.GraduationYear)
            .InclusiveBetween(1900, DateTime.Today.Year)
            .When(x => x.GraduationYear.HasValue)
            .WithMessage("El anio de recibido no es valido.");
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
                    .WithMessage("El dia de disponibilidad no es valido.");
                availability
                    .RuleFor(x => x.TimeSlot)
                    .InclusiveBetween(1, 4)
                    .WithMessage("La franja horaria no es valida.");
            });
        RuleFor(x => x.PatientGroups)
            .Must(groups => groups is null || groups.All(group => group >= 1 && group <= 5))
            .WithMessage("Los tipos de pacientes seleccionados son invalidos.");
        RuleForEach(x => x.Trainings)
            .ChildRules(training =>
            {
                training.RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
                training.RuleFor(x => x.Institution).MaximumLength(200);
                training
                    .RuleFor(x => x.Year)
                    .InclusiveBetween(1900, DateTime.Today.Year)
                    .When(x => x.Year.HasValue)
                    .WithMessage("El anio del curso no es valido.");
                training.RuleFor(x => x.Description).MaximumLength(500);
            });
    }

    private bool BeValidAppointmentType(string type)
    {
        return type == "Presencial" || type == "Online" || type == "Ambos";
    }
}
