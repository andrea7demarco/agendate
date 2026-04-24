using FluentValidation;

namespace WebApi.Features.Professionals.UseCases.CreateProfessional;

public class CreateProfessionalRequestValidator : AbstractValidator<CreateProfessionalRequest>
{
    public CreateProfessionalRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.ConsultationCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AppointmentType)
            .Must(BeValidAppointmentType)
            .WithMessage("AppointmentType debe ser 'Presencial', 'Online' o 'Ambos'.");
    }

    private bool BeValidAppointmentType(string type)
    {
        return type == "Presencial" || type == "Online" || type == "Ambos";
    }
}
