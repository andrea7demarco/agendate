using Microsoft.EntityFrameworkCore;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;
using WebApi.Shared.Utils;

namespace WebApi.Features.Profiles.UseCases.GetMyProfile;

public class GetMyProfileHandler
{
    private readonly IApplicationDbContext _context;

    public GetMyProfileHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MyProfileResponse>> HandleAsync(
        string applicationUserId,
        CancellationToken ct
    )
    {
        var patient = await _context
            .Patients.AsNoTracking()
            .Include(p => p.PatientHealthInsurances)
                .ThenInclude(phi => phi.HealthInsurance)
            .FirstOrDefaultAsync(p => p.ApplicationUserId == applicationUserId, ct);

        if (patient is not null)
        {
            var patientProfile = new
            {
                patient.Id,
                patient.ApplicationUserId,
                patient.FirstName,
                patient.LastName,
                FullName = $"{patient.FirstName} {patient.LastName}".Trim(),
                patient.Email,
                Gender = patient.Gender.ToString(),
                patient.BirthDate,
                Age = AgeCalculator.Calculate(patient.BirthDate),
                HealthInsurances = patient.PatientHealthInsurances.Select(phi => new
                {
                    phi.HealthInsurance.Id,
                    phi.HealthInsurance.Name,
                    phi.HealthInsurance.Acronym,
                    phi.AffiliateNumber,
                    phi.PlanName,
                }),
            };

            return Result<MyProfileResponse>.Success(
                new MyProfileResponse("paciente", patientProfile)
            );
        }

        var professional = await _context
            .Professionals.AsNoTracking()
            .Include(p => p.ProfessionalSpecialties)
                .ThenInclude(ps => ps.Specialty)
            .Include(p => p.ProfessionalHealthInsurances)
                .ThenInclude(phi => phi.HealthInsurance)
            .FirstOrDefaultAsync(p => p.ApplicationUserId == applicationUserId, ct);

        if (professional is not null)
        {
            var professionalProfile = new
            {
                professional.Id,
                professional.ApplicationUserId,
                professional.FirstName,
                professional.LastName,
                FullName = $"{professional.FirstName} {professional.LastName}".Trim(),
                professional.Email,
                professional.Dni,
                professional.PhoneNumber,
                professional.ConsultationCost,
                AppointmentType = professional.AppointmentType.ToString(),
                professional.Address,
                professional.Province,
                professional.NationalLicense,
                professional.ProvincialLicense,
                professional.Biography,
                Specialties = professional.ProfessionalSpecialties.Select(ps => new
                {
                    ps.Specialty.Id,
                    ps.Specialty.Name,
                }),
                HealthInsurances = professional.ProfessionalHealthInsurances.Select(phi => new
                {
                    phi.HealthInsurance.Id,
                    phi.HealthInsurance.Name,
                    phi.HealthInsurance.Acronym,
                }),
            };

            return Result<MyProfileResponse>.Success(
                new MyProfileResponse("profesional", professionalProfile)
            );
        }

        return Result<MyProfileResponse>.Failure(
            Error.NotFound("profile.not_found", "El usuario no tiene un perfil asociado.")
        );
    }
}
