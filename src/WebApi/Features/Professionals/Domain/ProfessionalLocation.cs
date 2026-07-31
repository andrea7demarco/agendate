namespace WebApi.Features.Professionals.Domain;

public class ProfessionalLocation
{
    public int Id { get; set; }

    public int ProfessionalId { get; set; }
    public Professional Professional { get; set; } = null!;

    public string Name { get; set; } = string.Empty; // nombre del lugar ej, centro terapeutico san miguel

    public string? FormattedAddress { get; set; }
    public string? Street { get; set; }
    public string? StreetNumber { get; set; }
    public string? PostalCode { get; set; }

    public string? City { get; set; }
    public string? Province { get; set; }

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public string? ExternalPlaceId { get; set; }
    public string? ExternalProvider { get; set; }

    public string? Instructions { get; set; }

    public bool IsActive { get; set; } = true;
}
