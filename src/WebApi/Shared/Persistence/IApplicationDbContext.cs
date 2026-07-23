using Microsoft.EntityFrameworkCore;
using WebApi.Features.HealthInsurances.Domain;
using WebApi.Features.Identity.Domain;
using WebApi.Features.Patients.Domain;
using WebApi.Features.People.Domain;
using WebApi.Features.Professionals.Domain;

namespace WebApi.Shared.Persistence;

public interface IApplicationDbContext
{
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Person> People { get; }
    DbSet<Professional> Professionals { get; }

    DbSet<Specialty> Specialties { get; }
    DbSet<ProfessionalSpecialty> ProfessionalSpecialties { get; }
    DbSet<ProfessionalLocation> ProfessionalLocations { get; }
    DbSet<Patient> Patients { get; }
    DbSet<HealthInsurance> HealthInsurances { get; }
    DbSet<ProfessionalHealthInsurance> ProfessionalHealthInsurances { get; }
    DbSet<PatientHealthInsurance> PatientHealthInsurances { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}
