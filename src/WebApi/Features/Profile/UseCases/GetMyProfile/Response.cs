using WebApi.Shared.Utils;

namespace WebApi.Features.Profiles.UseCases.GetMyProfile;

public record MyProfileResponse(string ProfileType, object Profile);

public record PatientProfileResponse(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string Gender,
    DateOnly BirthDate,
    AgeInfo Age,
    List<PatientHealthInsuranceProfileResponse> HealthInsurances
);

public record ProfessionalProfileResponse(
    int Id,
    string FirstName,
    string LastName,
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
    List<SpecialtyProfileResponse> Specialties,
    List<ProfessionalHealthInsuranceProfileResponse> HealthInsurances
);

public record SpecialtyProfileResponse(int Id, string Name);

public record PatientHealthInsuranceProfileResponse(
    int Id,
    string Name,
    string Acronym,
    string? AffiliateNumber,
    string? PlanName
);

public record ProfessionalHealthInsuranceProfileResponse(int Id, string Name, string Acronym);
