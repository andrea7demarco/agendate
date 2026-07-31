namespace WebApi.Features.Professionals.Domain;

public class ProfessionalTraining
{
    public int Id { get; set; }

    public int ProfessionalId { get; set; }
    public Professional Professional { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string? Institution { get; set; }
    public int? Year { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
