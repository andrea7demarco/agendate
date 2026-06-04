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
