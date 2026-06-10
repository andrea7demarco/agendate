namespace WebApi.Features.Professionals.Domain;

public class ProfessionalLocation
{
    public int Id { get; set; }

    public int ProfessionalId { get; set; }
    public Professional Professional { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public LocationType Type { get; set; }

    public string Street { get; set; } = string.Empty;
    public string StreetNumber { get; set; } = string.Empty;
    public string? Floor { get; set; }
    public string? Office { get; set; }
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string? PostalCode { get; set; }

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public string? GooglePlaceId { get; set; }
    public string? Instructions { get; set; }

    public bool IsActive { get; set; } = true;
}
