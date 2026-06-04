using Microsoft.EntityFrameworkCore;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.GetProfessionalById;

public class GetProfessionalByIdHandler
{
    private readonly IApplicationDbContext _context; //unidad de trabajo con la bd (se inyecta en el constructor) dependecy injection

    public GetProfessionalByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProfessionalResponse>> HandleAsync(int id, CancellationToken ct) // ese cancellation token es para cancelar la op si el cliente se desconecta
    {
        var professional = await _context
            .Professionals // _context.Professionals es el DbSet<Professional> que representa la tabla de profesionales en la base de datos (hacemos consulta con LINQ)
            .Where(p => p.Id == id)
            .Select(p => new ProfessionalResponse(
                p.Id,
                p.ApplicationUserId,
                (p.FirstName + " " + p.LastName).Trim(),
                p.Dni,
                p.PhoneNumber,
                p.Email,
                p.ConsultationCost,
                p.AppointmentType.ToString(),
                p.Address,
                p.Province,
                p.NationalLicense,
                p.ProvincialLicense,
                p.Biography,
                p.ProfessionalSpecialties.Select(ps => new SpecialtyResponse(
                        ps.Specialty.Id,
                        ps.Specialty.Name,
                        ps.Specialty.ParentSpecialtyId
                    ))
                    .ToList()
            ))
            .FirstOrDefaultAsync(ct); //ejecuta la consulta en postgresql y devuelve el primer resultado , o null si no existe

        if (professional is null) //si no se encuentra el prof , retorna un resultado de fallo usando la cloase Result<T>
            return Result<ProfessionalResponse>.Failure(
                Error.NotFound(
                    "professional.not_found",
                    $"No se encontró un profesional con ID {id}."
                )
            );

        return Result<ProfessionalResponse>.Success(professional);
    }
}
