namespace WebApi.Features.Professionals.Domain;

public class HealthInsurance
{
    public int Id { get; set; }
    public string ShortName { get; set; } = string.Empty; // siglas

    public virtual ICollection<ProfessionalHealthInsurance> ProfessionalHealthInsurances { get; set; } =
    [];
}
