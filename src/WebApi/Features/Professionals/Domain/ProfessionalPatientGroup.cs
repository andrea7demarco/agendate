namespace WebApi.Features.Professionals.Domain;

public class ProfessionalPatientGroup
{
    public int ProfessionalId { get; set; }
    public Professional Professional { get; set; } = null!;

    public PatientGroup PatientGroup { get; set; }
}
