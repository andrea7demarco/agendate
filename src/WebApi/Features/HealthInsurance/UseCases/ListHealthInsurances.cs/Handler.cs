using Microsoft.EntityFrameworkCore;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;

namespace WebApi.Features.HealthInsurances.UseCases.ListHealthInsurances;

public class ListHealthInsurancesHandler
{
    private readonly IApplicationDbContext _context;

    public ListHealthInsurancesHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<HealthInsuranceResponse>>> HandleAsync(CancellationToken ct)
    {
        var healthInsurances = await _context
            .HealthInsurances.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new HealthInsuranceResponse(x.Id, x.Name, x.Acronym))
            .ToListAsync(ct);

        return Result<List<HealthInsuranceResponse>>.Success(healthInsurances);
    }
}
