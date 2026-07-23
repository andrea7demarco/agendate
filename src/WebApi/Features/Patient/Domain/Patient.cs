using WebApi.Features.People.Domain;

namespace WebApi.Features.Patients.Domain;

public class Patient : Person
{
    public DateOnly BirthDate { get; set; }
    public Gender Gender { get; set; }
    public bool HasCud { get; set; }
    public ICollection<PatientHealthInsurance> PatientHealthInsurances { get; set; } = [];
}
