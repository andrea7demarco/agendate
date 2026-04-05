using Microsoft.EntityFrameworkCore;
using WebApi.Features.Identity.Domain;

namespace WebApi.Shared.Persistence;

public interface IApplicationDbContext
{
    DbSet<RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}
