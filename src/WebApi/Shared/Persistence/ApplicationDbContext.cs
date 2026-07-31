using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.HealthInsurances.Domain;
using WebApi.Features.Identity.Domain;
using WebApi.Features.Patients.Domain;
using WebApi.Features.People.Domain;
using WebApi.Features.Professionals.Domain;

namespace WebApi.Shared.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options),
        IApplicationDbContext
{
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Person> People => Set<Person>();

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientHealthInsurance> PatientHealthInsurances => Set<PatientHealthInsurance>();

    public DbSet<Professional> Professionals => Set<Professional>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<ProfessionalSpecialty> ProfessionalSpecialties => Set<ProfessionalSpecialty>();
    public DbSet<ProfessionalLocation> ProfessionalLocations => Set<ProfessionalLocation>();
    public DbSet<ProfessionalAvailability> ProfessionalAvailabilities =>
        Set<ProfessionalAvailability>();
    public DbSet<ProfessionalTraining> ProfessionalTrainings => Set<ProfessionalTraining>();
    public DbSet<ProfessionalPatientGroup> ProfessionalPatientGroups =>
        Set<ProfessionalPatientGroup>();
    public DbSet<ProfessionalHealthInsurance> ProfessionalHealthInsurances =>
        Set<ProfessionalHealthInsurance>();

    public DbSet<HealthInsurance> HealthInsurances => Set<HealthInsurance>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.Entity<Person>(e =>
        {
            e.HasIndex(x => x.ApplicationUserId).IsUnique();

            e.HasOne(x => x.ApplicationUser)
                .WithOne()
                .HasForeignKey<Person>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Specialty>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Name).HasMaxLength(120).IsRequired();

            e.HasOne(x => x.ParentSpecialty)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentSpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ProfessionalSpecialty>(e =>
        {
            e.HasKey(x => new { x.ProfessionalId, x.SpecialtyId });

            e.HasOne(x => x.Professional)
                .WithMany(x => x.ProfessionalSpecialties)
                .HasForeignKey(x => x.ProfessionalId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Specialty)
                .WithMany(x => x.ProfessionalSpecialties)
                .HasForeignKey(x => x.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ProfessionalLocation>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Name).HasMaxLength(150).IsRequired();

            e.Property(x => x.FormattedAddress).HasMaxLength(250);

            e.Property(x => x.Street).HasMaxLength(150);

            e.Property(x => x.StreetNumber).HasMaxLength(20);

            e.Property(x => x.City).HasMaxLength(100);

            e.Property(x => x.Province).HasMaxLength(100);

            e.Property(x => x.PostalCode).HasMaxLength(20);

            e.Property(x => x.Latitude).HasPrecision(9, 6);

            e.Property(x => x.Longitude).HasPrecision(9, 6);

            e.Property(x => x.ExternalPlaceId).HasMaxLength(250);

            e.Property(x => x.ExternalProvider).HasMaxLength(50);

            e.Property(x => x.Instructions).HasMaxLength(500);

            e.HasOne(x => x.Professional)
                .WithMany(x => x.Locations)
                .HasForeignKey(x => x.ProfessionalId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => x.ProfessionalId);
        });

        builder.Entity<Professional>(e =>
        {
            e.Property(x => x.DegreeTitle).HasMaxLength(150);

            e.Property(x => x.University).HasMaxLength(150);
        });

        builder.Entity<ProfessionalAvailability>(e =>
        {
            e.HasKey(x => new
            {
                x.ProfessionalId,
                x.DayOfWeek,
                x.TimeSlot,
            });

            e.Property(x => x.DayOfWeek).HasConversion<int>();

            e.Property(x => x.TimeSlot).HasConversion<int>();

            e.HasOne(x => x.Professional)
                .WithMany(x => x.Availabilities)
                .HasForeignKey(x => x.ProfessionalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProfessionalTraining>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Title).HasMaxLength(200).IsRequired();

            e.Property(x => x.Institution).HasMaxLength(200);

            e.Property(x => x.Description).HasMaxLength(500);

            e.HasOne(x => x.Professional)
                .WithMany(x => x.Trainings)
                .HasForeignKey(x => x.ProfessionalId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => x.ProfessionalId);
        });

        builder.Entity<ProfessionalPatientGroup>(e =>
        {
            e.HasKey(x => new { x.ProfessionalId, x.PatientGroup });

            e.Property(x => x.PatientGroup).HasConversion<int>();

            e.HasOne(x => x.Professional)
                .WithMany(x => x.PatientGroups)
                .HasForeignKey(x => x.ProfessionalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<HealthInsurance>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Name).HasMaxLength(150).IsRequired();

            e.Property(x => x.Acronym).HasMaxLength(30).IsRequired();

            e.HasIndex(x => x.Name).IsUnique();

            e.HasIndex(x => x.Acronym);
        });

        builder.Entity<PatientHealthInsurance>(e =>
        {
            e.HasKey(x => new { x.PatientId, x.HealthInsuranceId });

            e.Property(x => x.AffiliateNumber).HasMaxLength(80);

            e.Property(x => x.PlanName).HasMaxLength(100);

            e.HasOne(x => x.Patient)
                .WithMany(x => x.PatientHealthInsurances)
                .HasForeignKey(x => x.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.HealthInsurance)
                .WithMany(x => x.PatientHealthInsurances)
                .HasForeignKey(x => x.HealthInsuranceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ProfessionalHealthInsurance>(e =>
        {
            e.HasKey(x => new { x.ProfessionalId, x.HealthInsuranceId });

            e.HasOne(x => x.Professional)
                .WithMany(x => x.ProfessionalHealthInsurances)
                .HasForeignKey(x => x.ProfessionalId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.HealthInsurance)
                .WithMany(x => x.ProfessionalHealthInsurances)
                .HasForeignKey(x => x.HealthInsuranceId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
