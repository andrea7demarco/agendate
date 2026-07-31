using WebApi.Features.People.Domain;

namespace WebApi.Features.Professionals.Domain;

public class Professional : Person
{
    public decimal ConsultationCost { get; set; }
    public AppointmentType AppointmentType { get; set; }
    public string? Address { get; set; }
    public string? Province { get; set; }
    public string? NationalLicense { get; set; }
    public string? ProvincialLicense { get; set; }
    public string? Biography { get; set; }
    public string? DegreeTitle { get; set; }
    public string? University { get; set; }
    public int? GraduationYear { get; set; }

    public ICollection<ProfessionalHealthInsurance> ProfessionalHealthInsurances { get; set; } = [];
    public ICollection<ProfessionalSpecialty> ProfessionalSpecialties { get; set; } = [];
    public ICollection<ProfessionalLocation> Locations { get; set; } = [];
    public ICollection<ProfessionalAvailability> Availabilities { get; set; } = [];
    public ICollection<ProfessionalTraining> Trainings { get; set; } = [];
    public ICollection<ProfessionalPatientGroup> PatientGroups { get; set; } = [];
}
