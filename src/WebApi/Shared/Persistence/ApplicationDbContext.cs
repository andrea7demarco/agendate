using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.Identity.Domain;
using WebApi.Features.People.Domain;
using WebApi.Features.Professionals.Domain;

namespace WebApi.Shared.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options),
        IApplicationDbContext
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Person> People => Set<Person>();
    public DbSet<Professional> Professionals => Set<Professional>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<ProfessionalSpecialty> ProfessionalSpecialties => Set<ProfessionalSpecialty>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.Entity<Specialty>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(120).IsRequired();

            e.HasOne(x => x.ParentSpecialty)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentSpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Professional>(e =>
        {
            e.HasIndex(x => x.ApplicationUserId).IsUnique();

            e.HasOne(x => x.ApplicationUser)
                .WithOne()
                .HasForeignKey<Professional>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ProfessionalSpecialty>(e =>
        {
            e.HasKey(x => new { x.ProfessionalId, x.SpecialtyId });

            e.HasOne(x => x.Professional)
                .WithMany(x => x.ProfessionalSpecialties)
                .HasForeignKey(x => x.ProfessionalId);

            e.HasOne(x => x.Specialty)
                .WithMany(x => x.ProfessionalSpecialties)
                .HasForeignKey(x => x.SpecialtyId);
        });
    }
}
