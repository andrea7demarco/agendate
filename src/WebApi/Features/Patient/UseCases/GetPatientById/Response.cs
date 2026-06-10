using WebApi.Shared.Utils;

namespace WebApi.Features.Patients.UseCases.GetPatientById;

public record PatientResponse(
    int Id,
    string? ApplicationUserId,
    string FullName,
    string Email,
    string Gender,
    DateOnly BirthDate,
    AgeInfo Age
);

//record tiene propiedades de solo lectura
//record es inmutable por defecto, lo que significa que una vez creado un objeto de tipo record, sus propiedades no pueden ser modificadas.
// Esto es útil para garantizar la integridad de los datos y evitar efectos secundarios no deseados.
