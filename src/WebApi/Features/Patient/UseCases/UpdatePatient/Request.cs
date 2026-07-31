namespace WebApi.Features.Patients.UseCases.UpdatePatient;

public record UpdatePatientRequest(
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    int Gender,
    bool HasCud,
    List<PatientHealthInsuranceRequest>? HealthInsurances
);

public record PatientHealthInsuranceRequest(
    int HealthInsuranceId,
    string? AffiliateNumber,
    string? PlanName
);
