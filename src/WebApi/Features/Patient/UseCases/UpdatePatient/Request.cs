namespace WebApi.Features.Patients.UseCases.UpdatePatient;

public record UpdatePatientRequest(
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    int Gender
);
