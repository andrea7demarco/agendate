using Microsoft.EntityFrameworkCore;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;
using WebApi.Shared.Utils;

namespace WebApi.Features.Patients.UseCases.ListPatients;

public class ListPatientsHandler
{
    private readonly IApplicationDbContext _context;

    public ListPatientsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<PatientResponse>>> HandleAsync(CancellationToken ct)
    {
        // Primero traigo los pacientes desde Postgres
        var patients = await _context.Patients.AsNoTracking().ToListAsync(ct);

        // Después calculo la edad (no en la bd)
        var response = patients
            .Select(p => new PatientResponse(
                p.Id,
                p.ApplicationUserId,
                $"{p.FirstName} {p.LastName}",
                p.Email,
                p.Gender.ToString(),
                p.BirthDate,
                AgeCalculator.Calculate(p.BirthDate)
            ))
            .ToList();

        return Result<List<PatientResponse>>.Success(response);
    }
}
