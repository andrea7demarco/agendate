using WebApi.Shared.Utils;

namespace WebApi.Features.Profiles.UseCases.GetMyProfile;

public record MyProfileResponse(string ProfileType, object Profile);

public record PatientProfileResponse(
    int Id,
    string FullName,
    string Email,
    string Gender,
    DateOnly BirthDate,
    AgeInfo Age
);

public record ProfessionalProfileResponse(
    int Id,
    string FullName,
    string Email,
    string Dni,
    string PhoneNumber,
    decimal ConsultationCost,
    string AppointmentType,
    string? Address,
    string Province,
    string NationalLicense,
    string ProvincialLicense,
    string? Biography,
    List<SpecialtyProfileResponse> Specialties
);

public record SpecialtyProfileResponse(int Id, string Name);
