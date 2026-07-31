using FluentValidation;

namespace WebApi.Features.Patients.UseCases.UpdatePatient;

public class UpdatePatientRequestValidator : AbstractValidator<UpdatePatientRequest>
{
    public UpdatePatientRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .Must(date => date < DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("La fecha de nacimiento debe ser anterior a hoy.");
        RuleFor(x => x.Gender)
            .InclusiveBetween(1, 3)
            .WithMessage("El genero debe ser Masculino, Femenino u Otro.");
        RuleFor(x => x.HealthInsurances)
            .Must(items => items is null || items.All(item => item.HealthInsuranceId > 0))
            .WithMessage("Las obras sociales seleccionadas son invalidas.");
        RuleForEach(x => x.HealthInsurances)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.AffiliateNumber).MaximumLength(80);
                item.RuleFor(x => x.PlanName).MaximumLength(100);
            });
    }
}
