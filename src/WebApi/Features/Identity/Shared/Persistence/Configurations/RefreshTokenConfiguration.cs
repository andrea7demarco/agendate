using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Features.Identity.Domain;

namespace WebApi.Features.Identity.Shared.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshToken");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.TokenHash).IsUnique();

        builder.Property(x => x.TokenHash).HasMaxLength(200).IsRequired();

        builder.Property(x => x.UserId).IsRequired();

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
