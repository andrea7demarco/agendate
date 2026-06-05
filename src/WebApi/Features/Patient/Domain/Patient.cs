using WebApi.Features.People.Domain;

namespace WebApi.Features.Patients.Domain;

public class Patient : Person
{
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
}
