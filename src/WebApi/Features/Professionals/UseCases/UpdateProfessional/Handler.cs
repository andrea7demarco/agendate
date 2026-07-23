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
            .FirstOrDefaultAsync(p => p.Id == id && p.ApplicationUserId == applicationUserId, ct);

        if (professional is null)
            return Result<UpdateProfessionalResponse>.Failure(
                Error.NotFound(
                    "professional.not_found",
                    $"No se encontró un profesional con ID {id}."
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
                Error.BadRequest("Hay especialidades inválidas en SpecialtyIds.")
            );

        professional.FirstName = request.FirstName;
        professional.LastName = request.LastName;
        professional.Dni = request.Dni;
        professional.PhoneNumber = request.PhoneNumber;
        professional.Email = request.Email;

        var appointmentType = request.AppointmentType switch
        {
            "Presencial" => AppointmentType.Presencial,
            "Online" => AppointmentType.Online,
            "Ambos" => AppointmentType.Ambos,
            _ => AppointmentType.Presencial,
        };

        professional.ConsultationCost = request.ConsultationCost;
        professional.AppointmentType = appointmentType;
        professional.Address = request.Address;
        professional.Province = request.Province;
        professional.NationalLicense = request.NationalLicense;
        professional.ProvincialLicense = request.ProvincialLicense;
        professional.Biography = request.Biography;

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
        await _context.SaveChangesAsync(ct);

        var response = new UpdateProfessionalResponse(
            professional.Id,
            $"{professional.FirstName} {professional.LastName}",
            professional.Email
        );

        return Result<UpdateProfessionalResponse>.Success(response);
    }
}
