using FluentValidation;

namespace WebApi.Features.Patients.UseCases.CreatePatient;

public class CreatePatientRequestValidator : AbstractValidator<CreatePatientRequest>
{
    public CreatePatientRequestValidator()
    {
        RuleFor(x => x.ApplicationUserId).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .LessThan(DateTime.Today)
            .WithMessage("La fecha de nacimiento debe ser anterior a hoy.");
        RuleFor(x => x.Gender)
            .InclusiveBetween(1, 3)
            .WithMessage("El genero debe ser Masculino, Femenino u Otro.");
    }
}
