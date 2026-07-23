using WebApi.Features.HealthInsurances.Domain;

namespace WebApi.Features.Patients.Domain;

public class PatientHealthInsurance
{
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int HealthInsuranceId { get; set; }
    public HealthInsurance HealthInsurance { get; set; } = null!;

    public string? AffiliateNumber { get; set; }
    public string? PlanName { get; set; }
}
