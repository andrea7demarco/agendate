namespace WebApi.Features.Professionals.UseCases.GetProfessionalById;

public record SpecialtyResponse(int Id, string Name, int? ParentSpecialtyId);

public record ProfessionalLocationResponse(
    int Id,
    string Name,
    string? FormattedAddress,
    string? City,
    string? Province,
    decimal? Latitude,
    decimal? Longitude,
    string? ExternalPlaceId,
    string? ExternalProvider,
    string? Instructions
);

public record ProfessionalAvailabilityResponse(
    int DayOfWeek,
    string DayName,
    int TimeSlot,
    string TimeSlotName
);

public record ProfessionalTrainingResponse(
    int Id,
    string Title,
    string? Institution,
    int? Year,
    string? Description
);

public record ProfessionalPatientGroupResponse(int Id, string Name);

public record ProfessionalResponse(
    int Id,
    string? ApplicationUserId,
    string FullName,
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
    List<SpecialtyResponse> Specialties,
    List<ProfessionalLocationResponse> Locations,
    List<ProfessionalAvailabilityResponse> Availabilities,
    List<ProfessionalPatientGroupResponse> PatientGroups,
    List<ProfessionalTrainingResponse> Trainings
);

//record tiene propiedades de solo lectura
//record es inmutable por defecto, lo que significa que una vez creado un objeto de tipo record, sus propiedades no pueden ser modificadas.
// Esto es útil para garantizar la integridad de los datos y evitar efectos secundarios no deseados.
