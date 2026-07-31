namespace WebApi.Features.Professionals.Domain;

public class ProfessionalAvailability
{
    public int ProfessionalId { get; set; }
    public Professional Professional { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }
    public ProfessionalTimeSlot TimeSlot { get; set; }
}
