namespace WebApi.Features.Patients.UseCases.CreatePatient;

public record CreatePatientRequest(
    string ApplicationUserId,
    DateOnly BirthDate,
    int Gender,
    string Email,
    string FirstName,
    string LastName,
    bool HasCud,
    List<PatientHealthInsuranceRequest> HealthInsurances
);

public record PatientHealthInsuranceRequest(
    int HealthInsuranceId,
    string? AffiliateNumber,
    string? PlanName
);
