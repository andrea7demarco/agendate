namespace WebApi.Features.Professionals.UseCases.UpdateProfessional;

public record UpdateProfessionalRequest(
    string FirstName,
    string LastName,
    string? Dni,
    string? PhoneNumber,
    string Email,
    decimal ConsultationCost,
    string AppointmentType,
    string? Address,
    string? Province,
    string? NationalLicense,
    string? ProvincialLicense,
    string? Biography,
    List<int> SpecialtyIds
);
