using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.Identity.Domain;
using WebApi.Features.People.Domain;
using WebApi.Features.Professionals.Domain;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Person> People { get; set; }
    public DbSet<Professional> Professionals { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configurar TPT
        builder.Entity<Person>().ToTable("People");
        builder.Entity<Professional>().ToTable("Professionals");

        // Configuraciones adicionales...
        builder.Entity<Person>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(p => p.Email).IsUnique();
        });

        builder.Entity<Professional>(entity =>
        {
            entity.Property(p => p.ConsultationCost).HasPrecision(10, 2);
            entity.Property(p => p.AppointmentType).HasConversion<string>().HasMaxLength(20);
        });
    }
}
