using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.Identity.Domain;
using WebApi.Features.Patients.Domain;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;

namespace WebApi.Features.Patients.UseCases.CreatePatient;

public class CreatePatientHandler
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreatePatientHandler(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager
    )
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Result<CreatePatientResponse>> HandleAsync(
        CreatePatientRequest request,
        CancellationToken ct
    )
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<CreatePatientResponse>.Failure(
                Error.BadRequest("El email es obligatorio.")
            );

        var user = await _userManager.FindByIdAsync(request.ApplicationUserId);
        if (user is null)
            return Result<CreatePatientResponse>.Failure(
                Error.NotFound(
                    "user.not_found",
                    $"No se encontró un usuario con ID {request.ApplicationUserId}."
                )
            );

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            return Result<CreatePatientResponse>.Failure(
                Error.BadRequest("El email del paciente debe coincidir con el usuario registrado.")
            );

        var existingPatientForUser = await _context.Patients.AnyAsync(
            p => p.ApplicationUserId == request.ApplicationUserId,
            ct
        );
        if (existingPatientForUser)
            return Result<CreatePatientResponse>.Failure(
                Error.Conflict(
                    "patient.user_already_has_profile",
                    "Este usuario ya tiene un perfil de paciente."
                )
            );

        var existingPerson = await _context.People.FirstOrDefaultAsync(
            p => p.Email == request.Email,
            ct
        );
        if (existingPerson != null)
            return Result<CreatePatientResponse>.Failure(
                Error.Conflict("person.email_exists", "Ya existe una persona con ese email.")
            );

        var patient = new Patient
        {
            ApplicationUserId = request.ApplicationUserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            BirthDate = request.BirthDate,
            Gender = (Gender)request.Gender,
            HasCud = request.HasCud,
        };

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
            return Result<CreatePatientResponse>.Failure(
                Error.BadRequest("Hay obras sociales invalidas.")
            );
        }

        patient.PatientHealthInsurances = healthInsurances
            .GroupBy(x => x.HealthInsuranceId)
            .Select(group =>
            {
                var item = group.First();

                return new PatientHealthInsurance
                {
                    Patient = patient,
                    HealthInsuranceId = item.HealthInsuranceId,
                    AffiliateNumber = item.AffiliateNumber,
                    PlanName = item.PlanName,
                };
            })
            .ToList();

        _context.Patients.Add(patient);

        await _context.SaveChangesAsync(ct);

        user.RegistrationCompleted = true;
        await _userManager.UpdateAsync(user);

        var response = new CreatePatientResponse(
            patient.Id,
            $"{patient.FirstName} {patient.LastName}",
            patient.Email
        );

        return Result<CreatePatientResponse>.Success(response);
    }
}
