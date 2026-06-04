using FluentValidation;

namespace WebApi.Features.Professionals.UseCases.UpdateProfessional;

public class UpdateProfessionalRequestValidator : AbstractValidator<UpdateProfessionalRequest>
{
    public UpdateProfessionalRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.ConsultationCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Province).MaximumLength(80);
        RuleFor(x => x.ProvincialLicense)
            .Matches(@"^\d+$")
            .When(x => !string.IsNullOrWhiteSpace(x.ProvincialLicense))
            .WithMessage("La matrícula provincial debe ser numérica.");
        RuleFor(x => x.AppointmentType)
            .Must(BeValidAppointmentType)
            .WithMessage("AppointmentType debe ser 'Presencial', 'Online' o 'Ambos'.");
        RuleFor(x => x.Biography).MaximumLength(200);
        RuleFor(x => x.SpecialtyIds).NotEmpty();
        RuleFor(x => x.SpecialtyIds)
            .NotNull()
            .Must(ids => ids.Count > 0)
            .WithMessage("Debes enviar al menos una especialidad.");
    }

    private bool BeValidAppointmentType(string type)
    {
        return type == "Presencial" || type == "Online" || type == "Ambos";
    }
}
