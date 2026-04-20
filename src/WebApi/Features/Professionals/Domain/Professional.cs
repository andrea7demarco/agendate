using WebApi.Features.People.Domain;

namespace WebApi.Features.Professionals.Domain;

public class Professional : Person // ← herencia
{
    public decimal ConsultationCost { get; set; }
    public AppointmentType AppointmentType { get; set; } // Presencial, Virtual, etc.??? algo mas
    public string? Address { get; set; }
    public string? NationalLicense { get; set; }
    public string? ProvincialLicense { get; set; }
}
