namespace WebApi.Features.Professionals.Domain;

public class ProfessionalHealthInsurance
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public int HealthInsuranceId { get; set; }

    public virtual Professional Professional { get; set; } = null!;
    public virtual HealthInsurance HealthInsurance { get; set; } = null!;
}
