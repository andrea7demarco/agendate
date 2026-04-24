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
        RuleFor(x => x.AppointmentType)
            .Must(BeValidAppointmentType)
            .WithMessage("AppointmentType debe ser 'Presencial', 'Online' o 'Ambos'.");
    }

    private bool BeValidAppointmentType(string type)
    {
        return type == "Presencial" || type == "Online" || type == "Ambos";
    }
}
