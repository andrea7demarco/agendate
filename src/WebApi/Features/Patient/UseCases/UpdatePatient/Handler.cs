using Microsoft.EntityFrameworkCore;
using WebApi.Features.Patients.Domain;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;

namespace WebApi.Features.Patients.UseCases.UpdatePatient;

public class UpdatePatientHandler
{
    private readonly IApplicationDbContext _context;

    public UpdatePatientHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UpdatePatientResponse>> HandleAsync(
        int id,
        string applicationUserId,
        UpdatePatientRequest request,
        CancellationToken ct
    )
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(
            p => p.Id == id && p.ApplicationUserId == applicationUserId,
            ct
        );

        if (patient is null)
            return Result<UpdatePatientResponse>.Failure(
                Error.NotFound("patient.not_found", "No se encontro el perfil del paciente.")
            );

        patient.FirstName = request.FirstName.Trim();
        patient.LastName = request.LastName.Trim();
        patient.BirthDate = request.BirthDate;
        patient.Gender = (Gender)request.Gender;

        await _context.SaveChangesAsync(ct);

        return Result<UpdatePatientResponse>.Success(
            new UpdatePatientResponse(
                patient.Id,
                $"{patient.FirstName} {patient.LastName}".Trim(),
                patient.Email
            )
        );
    }
}
