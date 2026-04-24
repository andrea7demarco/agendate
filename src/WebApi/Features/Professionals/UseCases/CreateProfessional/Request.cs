namespace WebApi.Features.Professionals.UseCases.CreateProfessional;

public record CreateProfessionalRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Dni,
    string? PhoneNumber,
    decimal ConsultationCost,
    string AppointmentType, // "Presencial", "Online" o "Ambos"
    string? Address,
    string? NationalLicense,
    string? ProvincialLicense
);
