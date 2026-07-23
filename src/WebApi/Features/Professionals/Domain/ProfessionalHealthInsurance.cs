using WebApi.Features.HealthInsurances.Domain;

namespace WebApi.Features.Professionals.Domain;

public class ProfessionalHealthInsurance
{
    public int ProfessionalId { get; set; }
    public Professional Professional { get; set; } = null!;

    public int HealthInsuranceId { get; set; }
    public HealthInsurance HealthInsurance { get; set; } = null!;

    public bool RequiresAuthorization { get; set; }
    public bool AcceptsReimbursement { get; set; }
}
