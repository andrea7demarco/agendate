namespace WebApi.Features.Professionals.UseCases.GetProfessionalById;

public record ProfessionalResponse(
    int Id,
    string FirstName,
    string LastName,
    string? Dni,
    string? PhoneNumber,
    string Email,
    decimal ConsultationCost,
    string AppointmentType,
    string? Address,
    string? NationalLicense,
    string? ProvincialLicense
);

//record tiene propiedades de solo lectura
//record es inmutable por defecto, lo que significa que una vez creado un objeto de tipo record, sus propiedades no pueden ser modificadas.
// Esto es útil para garantizar la integridad de los datos y evitar efectos secundarios no deseados.
