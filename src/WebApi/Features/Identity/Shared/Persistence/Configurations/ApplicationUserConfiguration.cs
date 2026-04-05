using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Features.Identity.Domain;

namespace WebApi.Features.Identity.Shared.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(a => a.FirstName).HasMaxLength(100);

        builder.Property(a => a.LastName).HasMaxLength(100);

        builder.Property(a => a.RegistrationCompleted).HasDefaultValue(false);
    }
}
