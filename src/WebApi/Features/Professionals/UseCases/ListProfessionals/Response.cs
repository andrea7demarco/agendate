namespace WebApi.Features.Professionals.UseCases.ListProfessionals;

public record ProfessionalResponse(
    int Id,
    string? ApplicationUserId,
    string FullName,
    string Email,
    string? PhoneNumber,
    decimal ConsultationCost,
    string AppointmentType,
    string? Address,
    string? Province,
    string? NationalLicense,
    string? ProvincialLicense,
    string? Biography,
    List<string> Specialties
);

public record PaginatedProfessionalsResponse(
    List<ProfessionalResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);
