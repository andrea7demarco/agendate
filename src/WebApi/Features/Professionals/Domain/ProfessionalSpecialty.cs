namespace WebApi.Features.Professionals.Domain;

public class ProfessionalSpecialty
{
    public int ProfessionalId { get; set; }
    public Professional Professional { get; set; } = null!;

    public int SpecialtyId { get; set; }
    public Specialty Specialty { get; set; } = null!;
}
