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
    string? DegreeTitle,
    string? University,
    int? GraduationYear,
    List<int> SpecialtyIds,
    List<int>? HealthInsuranceIds,
    List<ProfessionalAvailabilityRequest>? Availabilities,
    List<int>? PatientGroups,
    List<ProfessionalTrainingRequest>? Trainings
);

public record ProfessionalAvailabilityRequest(int DayOfWeek, int TimeSlot);

public record ProfessionalTrainingRequest(
    int? Id,
    string Title,
    string? Institution,
    int? Year,
    string? Description
);
