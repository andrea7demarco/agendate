using WebApi.Features.Patients.Domain;
using WebApi.Features.Professionals.Domain;

namespace WebApi.Features.HealthInsurances.Domain;

public class HealthInsurance
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Acronym { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<PatientHealthInsurance> PatientHealthInsurances { get; set; } = [];
    public ICollection<ProfessionalHealthInsurance> ProfessionalHealthInsurances { get; set; } = [];
}
