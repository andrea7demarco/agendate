using Microsoft.EntityFrameworkCore;
using WebApi.Shared.Persistence;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.ListProfessionals;

public class ListProfessionalsHandler
{
    private readonly IApplicationDbContext _context;

    public ListProfessionalsHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<List<ProfessionalResponse>>> HandleAsync(CancellationToken ct)
    {
        var professionals = await _context
            .Professionals.Select(p => new ProfessionalResponse(
                p.Id,
                p.FirstName + " " + p.LastName,
                p.Email,
                p.PhoneNumber,
                p.ConsultationCost,
                p.AppointmentType.ToString(),
                p.Address,
                p.NationalLicense,
                p.ProvincialLicense
            ))
            .ToListAsync(ct);
        return Result<List<ProfessionalResponse>>.Success(professionals);
    }
}
