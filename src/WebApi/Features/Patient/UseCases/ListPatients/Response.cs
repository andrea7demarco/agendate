using WebApi.Shared.Utils;

namespace WebApi.Features.Patients.UseCases.ListPatients;

public record PatientResponse(
    int Id,
    string? ApplicationUserId,
    string FullName,
    string Email,
    string Gender,
    DateOnly BirthDate,
    AgeInfo Age
);
