namespace WebApi.Features.Professionals.UseCases.ListProfessionals;

public record ProfessionalResponse(
    int Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    decimal ConsultationCost,
    string AppointmentType,
    string? Address,
    string? NationalLicense,
    string? ProvincialLicense
);
