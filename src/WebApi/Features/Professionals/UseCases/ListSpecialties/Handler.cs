using Microsoft.EntityFrameworkCore;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.ListSpecialties;

public class ListSpecialtiesHandler
{
    private readonly IApplicationDbContext _context;

    public ListSpecialtiesHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<SpecialtyResponse>>> HandleAsync(CancellationToken ct)
    {
        var specialties = await _context
            .Specialties.OrderBy(s => s.ParentSpecialtyId.HasValue)
            .ThenBy(s => s.ParentSpecialtyId)
            .ThenBy(s => s.Name)
            .Select(s => new SpecialtyResponse(s.Id, s.Name, s.ParentSpecialtyId))
            .ToListAsync(ct);

        return Result<List<SpecialtyResponse>>.Success(specialties);
    }
}
