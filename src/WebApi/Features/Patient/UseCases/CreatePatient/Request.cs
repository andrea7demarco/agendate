namespace WebApi.Features.Patients.UseCases.CreatePatient;

public record CreatePatientRequest(
    string ApplicationUserId,
    DateTime BirthDate,
    int Gender,
    string Email,
    string FirstName,
    string LastName
);
