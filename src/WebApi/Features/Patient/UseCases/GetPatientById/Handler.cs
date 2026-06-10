using Microsoft.EntityFrameworkCore;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;
using WebApi.Shared.Utils;

namespace WebApi.Features.Patients.UseCases.GetPatientById;

public class GetPatientByIdHandler
{
    private readonly IApplicationDbContext _context;

    public GetPatientByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PatientResponse>> HandleAsync(int id, CancellationToken ct)
    {
        var patient = await _context
            .Patients.AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.ApplicationUserId,
                p.FirstName,
                p.LastName,
                p.Email,
                p.Gender,
                p.BirthDate,
            })
            .FirstOrDefaultAsync(ct);

        if (patient is null)
        {
            return Result<PatientResponse>.Failure(
                Error.NotFound("patient.not_found", $"No se encontró un paciente con ID {id}.")
            );
        }

        var response = new PatientResponse(
            patient.Id,
            patient.ApplicationUserId,
            $"{patient.FirstName} {patient.LastName}".Trim(),
            patient.Email,
            patient.Gender.ToString(),
            patient.BirthDate,
            AgeCalculator.Calculate(patient.BirthDate)
        );

        return Result<PatientResponse>.Success(response);
    }
}
