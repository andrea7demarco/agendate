using WebApi.Features.People.Domain;

namespace WebApi.Features.Patients.Domain;

public class Patient : Person
{
    public DateOnly BirthDate { get; set; }
    public Gender Gender { get; set; }
}
