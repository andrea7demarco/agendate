namespace WebApi.Features.Professionals.Domain;

public class Specialty
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // FK recursiva (padre)
    public int? ParentSpecialtyId { get; set; }
    public Specialty? ParentSpecialty { get; set; }

    // Hijos
    public ICollection<Specialty> Children { get; set; } = new List<Specialty>();

    // Relación N:N con Professional
    public ICollection<ProfessionalSpecialty> ProfessionalSpecialties { get; set; } =
        new List<ProfessionalSpecialty>();
}
