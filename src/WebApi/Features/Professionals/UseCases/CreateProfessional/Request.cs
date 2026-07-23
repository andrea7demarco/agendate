namespace WebApi.Features.Professionals.UseCases.CreateProfessional;

public record CreateProfessionalRequest(
    string ApplicationUserId,
    string FirstName,
    string LastName,
    string Email,
    string Dni,
    string PhoneNumber,
    decimal ConsultationCost,
    string AppointmentType, // "Presencial", "Online" o "Ambos"
    string? Address,
    string Province,
    string City,
    string NationalLicense,
    string ProvincialLicense,
    string? Biography,
    List<int> SpecialtyIds,
    List<int>? HealthInsuranceIds
);
