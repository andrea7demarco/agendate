namespace WebApi.Features.Professionals.UseCases.ListProfessionals;

public sealed record ListProfessionalsRequest(
    string? Search,
    string? Province,
    int? SpecialtyId,
    string? AppointmentType,
    int? Page,
    int? PageSize
);
