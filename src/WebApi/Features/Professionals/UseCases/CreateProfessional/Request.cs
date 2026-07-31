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
    string? DegreeTitle,
    string? University,
    int? GraduationYear,
    List<int> SpecialtyIds,
    List<int>? HealthInsuranceIds,
    List<ProfessionalLocationRequest>? Locations,
    List<ProfessionalAvailabilityRequest>? Availabilities,
    List<int>? PatientGroups,
    List<ProfessionalTrainingRequest>? Trainings
);

public record ProfessionalLocationRequest(
    string Name,
    string? FormattedAddress,
    string? Street,
    string? StreetNumber,
    string? City,
    string? Province,
    string? PostalCode,
    decimal? Latitude,
    decimal? Longitude,
    string? ExternalPlaceId,
    string? ExternalProvider,
    string? Instructions
);

public record ProfessionalAvailabilityRequest(int DayOfWeek, int TimeSlot);

public record ProfessionalTrainingRequest(
    string Title,
    string? Institution,
    int? Year,
    string? Description
);
