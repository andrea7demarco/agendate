using Microsoft.EntityFrameworkCore;
using WebApi.Features.Professionals.Domain;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.UpdateProfessional;

public class UpdateProfessionalHandler
{
    private readonly IApplicationDbContext _context;

    public UpdateProfessionalHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UpdateProfessionalResponse>> HandleAsync(
        int id,
        string applicationUserId,
        UpdateProfessionalRequest request,
        CancellationToken ct
    )
    {
        var professional = await _context
            .Professionals.Include(p => p.ProfessionalSpecialties)
            .Include(p => p.ProfessionalHealthInsurances)
            .Include(p => p.Availabilities)
            .Include(p => p.PatientGroups)
            .Include(p => p.Trainings)
            .FirstOrDefaultAsync(p => p.Id == id && p.ApplicationUserId == applicationUserId, ct);

        if (professional is null)
            return Result<UpdateProfessionalResponse>.Failure(
                Error.NotFound(
                    "professional.not_found",
                    $"No se encontro un profesional con ID {id}."
                )
            );

        var emailExists = await _context.People.AnyAsync(
            p => p.Email == request.Email && p.Id != id,
            ct
        );
        if (emailExists)
            return Result<UpdateProfessionalResponse>.Failure(
                Error.Conflict("person.email_exists", "Ya existe otra persona con ese email.")
            );

        var specialtyIds = request.SpecialtyIds.Distinct().ToList();
        var existingSpecialtyIds = await _context
            .Specialties.Where(s => specialtyIds.Contains(s.Id))
            .Select(s => s.Id)
            .ToListAsync(ct);

        if (existingSpecialtyIds.Count != specialtyIds.Count)
            return Result<UpdateProfessionalResponse>.Failure(
                Error.BadRequest("Hay especialidades invalidas en SpecialtyIds.")
            );

        var healthInsuranceIds = request.HealthInsuranceIds?.Distinct().ToList() ?? [];
        var availabilities =
            request.Availabilities?.DistinctBy(x => new { x.DayOfWeek, x.TimeSlot }).ToList() ?? [];
        var patientGroups = request.PatientGroups?.Distinct().ToList() ?? [];
        var trainings =
            request.Trainings?.Where(x => !string.IsNullOrWhiteSpace(x.Title)).ToList() ?? [];

        var existingHealthInsuranceIds = await _context
            .HealthInsurances.Where(x => healthInsuranceIds.Contains(x.Id) && x.IsActive)
            .Select(x => x.Id)
            .ToListAsync(ct);

        if (existingHealthInsuranceIds.Count != healthInsuranceIds.Count)
            return Result<UpdateProfessionalResponse>.Failure(
                Error.BadRequest("Hay obras sociales invalidas.")
            );

        professional.FirstName = request.FirstName;
        professional.LastName = request.LastName;
        professional.Dni = request.Dni;
        professional.PhoneNumber = request.PhoneNumber;
        professional.Email = request.Email;

        professional.ConsultationCost = request.ConsultationCost;
        professional.AppointmentType = request.AppointmentType switch
        {
            "Presencial" => AppointmentType.Presencial,
            "Online" => AppointmentType.Online,
            "Ambos" => AppointmentType.Ambos,
            _ => AppointmentType.Presencial,
        };
        professional.Address = request.Address;
        professional.Province = request.Province;
        professional.NationalLicense = request.NationalLicense;
        professional.ProvincialLicense = request.ProvincialLicense;
        professional.Biography = request.Biography;
        professional.DegreeTitle = request.DegreeTitle;
        professional.University = request.University;
        professional.GraduationYear = request.GraduationYear;

        professional.ProfessionalSpecialties.Clear();
        foreach (var specialtyId in specialtyIds)
        {
            professional.ProfessionalSpecialties.Add(
                new ProfessionalSpecialty
                {
                    ProfessionalId = professional.Id,
                    SpecialtyId = specialtyId,
                }
            );
        }

        var currentHealthInsurances = await _context
            .ProfessionalHealthInsurances.Where(x => x.ProfessionalId == professional.Id)
            .ToListAsync(ct);

        _context.ProfessionalHealthInsurances.RemoveRange(currentHealthInsurances);

        var newHealthInsurances = healthInsuranceIds.Select(
            healthInsuranceId => new ProfessionalHealthInsurance
            {
                ProfessionalId = professional.Id,
                HealthInsuranceId = healthInsuranceId,
            }
        );

        _context.ProfessionalHealthInsurances.AddRange(newHealthInsurances);

        var currentAvailabilities = await _context
            .ProfessionalAvailabilities.Where(x => x.ProfessionalId == professional.Id)
            .ToListAsync(ct);

        _context.ProfessionalAvailabilities.RemoveRange(currentAvailabilities);

        var newAvailabilities = availabilities.Select(availability => new ProfessionalAvailability
        {
            ProfessionalId = professional.Id,
            DayOfWeek = (DayOfWeek)availability.DayOfWeek,
            TimeSlot = (ProfessionalTimeSlot)availability.TimeSlot,
        });

        _context.ProfessionalAvailabilities.AddRange(newAvailabilities);

        var currentPatientGroups = await _context
            .ProfessionalPatientGroups.Where(x => x.ProfessionalId == professional.Id)
            .ToListAsync(ct);

        _context.ProfessionalPatientGroups.RemoveRange(currentPatientGroups);

        var newPatientGroups = patientGroups.Select(patientGroup => new ProfessionalPatientGroup
        {
            ProfessionalId = professional.Id,
            PatientGroup = (PatientGroup)patientGroup,
        });

        _context.ProfessionalPatientGroups.AddRange(newPatientGroups);

        var currentTrainings = await _context
            .ProfessionalTrainings.Where(x => x.ProfessionalId == professional.Id)
            .ToListAsync(ct);

        _context.ProfessionalTrainings.RemoveRange(currentTrainings);

        var newTrainings = trainings.Select(training => new ProfessionalTraining
        {
            ProfessionalId = professional.Id,
            Title = training.Title.Trim(),
            Institution = training.Institution,
            Year = training.Year,
            Description = training.Description,
        });

        _context.ProfessionalTrainings.AddRange(newTrainings);

        await _context.SaveChangesAsync(ct);

        var response = new UpdateProfessionalResponse(
            professional.Id,
            $"{professional.FirstName} {professional.LastName}",
            professional.Email
        );

        return Result<UpdateProfessionalResponse>.Success(response);
    }
}
