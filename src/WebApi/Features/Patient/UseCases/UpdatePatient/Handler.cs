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
        patient.HasCud = request.HasCud;

        var healthInsurances = request.HealthInsurances ?? [];
        var healthInsuranceIds = healthInsurances
            .Select(x => x.HealthInsuranceId)
            .Distinct()
            .ToList();

        var existingHealthInsuranceIds = await _context
            .HealthInsurances.Where(x => healthInsuranceIds.Contains(x.Id) && x.IsActive)
            .Select(x => x.Id)
            .ToListAsync(ct);

        if (existingHealthInsuranceIds.Count != healthInsuranceIds.Count)
        {
            return Result<UpdatePatientResponse>.Failure(
                Error.BadRequest("Hay obras sociales invalidas.")
            );
        }

        var currentHealthInsurances = await _context
            .PatientHealthInsurances.Where(x => x.PatientId == patient.Id)
            .ToListAsync(ct);

        var healthInsurancesToRemove = currentHealthInsurances
            .Where(current => !healthInsuranceIds.Contains(current.HealthInsuranceId))
            .ToList();

        _context.PatientHealthInsurances.RemoveRange(healthInsurancesToRemove);

        foreach (
            var item in healthInsurances.GroupBy(x => x.HealthInsuranceId).Select(x => x.First())
        )
        {
            var current = currentHealthInsurances.FirstOrDefault(x =>
                x.HealthInsuranceId == item.HealthInsuranceId
            );

            if (current is not null)
            {
                current.AffiliateNumber = item.AffiliateNumber;
                current.PlanName = item.PlanName;
                continue;
            }

            _context.PatientHealthInsurances.Add(
                new PatientHealthInsurance
                {
                    PatientId = patient.Id,
                    HealthInsuranceId = item.HealthInsuranceId,
                    AffiliateNumber = item.AffiliateNumber,
                    PlanName = item.PlanName,
                }
            );
        }

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
