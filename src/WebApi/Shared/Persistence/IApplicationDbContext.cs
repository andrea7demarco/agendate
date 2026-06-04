using Microsoft.EntityFrameworkCore;
using WebApi.Features.Identity.Domain;
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
    Task<int> SaveChangesAsync(CancellationToken ct);
}
