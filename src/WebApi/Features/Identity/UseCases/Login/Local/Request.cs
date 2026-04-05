using FluentValidation;

namespace WebApi.Features.Identity.UseCases.Login.Local;

public sealed record LocalLoginRequest(string? Email, string? Password);

public sealed class LocalLoginRequestValidator : AbstractValidator<LocalLoginRequest>
{
    public LocalLoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es requerido.")
            .EmailAddress()
            .WithMessage("El email no es válido.");

        RuleFor(x => x.Password).NotEmpty().WithMessage("La password es requerida.");
    }
}
