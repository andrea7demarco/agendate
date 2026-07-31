using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Features.Identity.Domain;
using WebApi.Features.Professionals.Domain;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.CreateProfessional;

public class CreateProfessionalHandler
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateProfessionalHandler(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager
    )
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Result<CreateProfessionalResponse>> HandleAsync(
        CreateProfessionalRequest request,
        CancellationToken ct
    )
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<CreateProfessionalResponse>.Failure(
                Error.BadRequest("El email es obligatorio.")
            );

        var user = await _userManager.FindByIdAsync(request.ApplicationUserId);
        if (user is null)
            return Result<CreateProfessionalResponse>.Failure(
                Error.NotFound(
                    "user.not_found",
                    $"No se encontró un usuario con ID {request.ApplicationUserId}."
                )
            );

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            return Result<CreateProfessionalResponse>.Failure(
                Error.BadRequest(
                    "El email del profesional debe coincidir con el usuario registrado."
                )
            );

        var existingProfessionalForUser = await _context.Professionals.AnyAsync(
            p => p.ApplicationUserId == request.ApplicationUserId,
            ct
        );
        if (existingProfessionalForUser)
            return Result<CreateProfessionalResponse>.Failure(
                Error.Conflict(
                    "professional.user_already_has_profile",
                    "Este usuario ya tiene un perfil profesional."
                )
            );

        var existingPerson = await _context.People.FirstOrDefaultAsync(
            p => p.Email == request.Email,
            ct
        );
        if (existingPerson != null)
            return Result<CreateProfessionalResponse>.Failure(
                Error.Conflict("person.email_exists", "Ya existe una persona con ese email.")
            );

        var specialtyIds = request.SpecialtyIds.Distinct().ToList();
        var existingSpecialtyIds = await _context
            .Specialties.Where(s => specialtyIds.Contains(s.Id))
            .Select(s => s.Id)
            .ToListAsync(ct);

        if (existingSpecialtyIds.Count != specialtyIds.Count)
            return Result<CreateProfessionalResponse>.Failure(
                Error.BadRequest("Hay especialidades inválidas en SpecialtyIds.")
            );

        var healthInsuranceIds = request.HealthInsuranceIds?.Distinct().ToList() ?? [];
        var locations = request.Locations ?? [];
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
        {
            return Result<CreateProfessionalResponse>.Failure(
                Error.BadRequest("Hay obras sociales invalidas.")
            );
        }

        var appointmentType = request.AppointmentType switch
        {
            "Presencial" => AppointmentType.Presencial,
            "Online" => AppointmentType.Online,
            "Ambos" => AppointmentType.Ambos,
            _ => AppointmentType.Presencial,
        };

        var professional = new Professional
        {
            ApplicationUserId = request.ApplicationUserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Dni = request.Dni,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            ConsultationCost = request.ConsultationCost,
            AppointmentType = appointmentType,
            Address = request.Address,
            Province = request.Province,
            NationalLicense = request.NationalLicense,
            ProvincialLicense = request.ProvincialLicense,
            Biography = request.Biography,
            DegreeTitle = request.DegreeTitle,
            University = request.University,
            GraduationYear = request.GraduationYear,
            ProfessionalHealthInsurances = healthInsuranceIds
                .Select(id => new ProfessionalHealthInsurance { HealthInsuranceId = id })
                .ToList(),
            Locations = locations
                .Select(location => new ProfessionalLocation
                {
                    Name = location.Name,
                    FormattedAddress = location.FormattedAddress,
                    Street = location.Street,
                    StreetNumber = location.StreetNumber,
                    City = location.City,
                    Province = location.Province,
                    PostalCode = location.PostalCode,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    ExternalPlaceId = location.ExternalPlaceId,
                    ExternalProvider = location.ExternalProvider,
                    Instructions = location.Instructions,
                })
                .ToList(),
            Availabilities = availabilities
                .Select(availability => new ProfessionalAvailability
                {
                    DayOfWeek = (DayOfWeek)availability.DayOfWeek,
                    TimeSlot = (ProfessionalTimeSlot)availability.TimeSlot,
                })
                .ToList(),
            PatientGroups = patientGroups
                .Select(patientGroup => new ProfessionalPatientGroup
                {
                    PatientGroup = (PatientGroup)patientGroup,
                })
                .ToList(),
            Trainings = trainings
                .Select(training => new ProfessionalTraining
                {
                    Title = training.Title.Trim(),
                    Institution = training.Institution,
                    Year = training.Year,
                    Description = training.Description,
                })
                .ToList(),
        };

        foreach (var specialtyId in specialtyIds)
        {
            professional.ProfessionalSpecialties.Add(
                new ProfessionalSpecialty { SpecialtyId = specialtyId }
            );
        }

        _context.Professionals.Add(professional);

        await _context.SaveChangesAsync(ct);

        user.RegistrationCompleted = true;
        await _userManager.UpdateAsync(user);

        var response = new CreateProfessionalResponse(
            professional.Id,
            $"{professional.FirstName} {professional.LastName}",
            professional.Email
        );

        return Result<CreateProfessionalResponse>.Success(response);
    }
}
