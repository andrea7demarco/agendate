namespace WebApi.Features.Professionals.UseCases.GetProfessionalById;

public record SpecialtyResponse(int Id, string Name, int? ParentSpecialtyId);

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
    List<SpecialtyResponse> Specialties
);

//record tiene propiedades de solo lectura
//record es inmutable por defecto, lo que significa que una vez creado un objeto de tipo record, sus propiedades no pueden ser modificadas.
// Esto es útil para garantizar la integridad de los datos y evitar efectos secundarios no deseados.
