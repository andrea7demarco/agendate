using Microsoft.EntityFrameworkCore;
using WebApi.Features.People.Domain;
using WebApi.Features.Professionals.Domain;
using WebApi.Shared.Persistence;
using WebApi.Shared.Results;

namespace WebApi.Features.Professionals.UseCases.CreateProfessional;

public class CreateProfessionalHandler
{
    private readonly IApplicationDbContext _context;

    public CreateProfessionalHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CreateProfessionalResponse>> HandleAsync(
        CreateProfessionalRequest request,
        CancellationToken ct
    )
    {
        //Verifica que el mial no sea nulo ni vacio
        if (string.IsNullOrWhiteSpace(request.Email))
            return Result<CreateProfessionalResponse>.Failure(
                Error.BadRequest("El email es obligatorio.")
            );

        // Verificar que no exista otro usuario con el mismo email
        var existingPerson = await _context.People.FirstOrDefaultAsync(
            p => p.Email == request.Email,
            ct
        );
        if (existingPerson != null)
            return Result<CreateProfessionalResponse>.Failure(
                Error.Conflict("person.email_exists", "Ya existe una persona con ese email.")
            );

        // Crear la persona
        var person = new Person
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Dni = request.Dni,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
        };

        // Convertir AppointmentType string a enum (si usas enum)
        var appointmentType = request.AppointmentType switch
        {
            "Presencial" => AppointmentType.Presencial,
            "Online" => AppointmentType.Online,
            "Ambos" => AppointmentType.Ambos,
            _ => AppointmentType.Presencial,
        };

        var professional = new Professional
        {
            // Propiedades heredadas de Person
            FirstName = request.FirstName,
            LastName = request.LastName,
            Dni = request.Dni,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            // Propiedades específicas de Professional
            ConsultationCost = request.ConsultationCost,
            AppointmentType = appointmentType,
            Address = request.Address,
            NationalLicense = request.NationalLicense,
            ProvincialLicense = request.ProvincialLicense,
        };

        _context.People.Add(person);
        _context.Professionals.Add(professional);

        await _context.SaveChangesAsync(ct);

        var response = new CreateProfessionalResponse(
            professional.Id,
            $"{person.FirstName} {person.LastName}",
            person.Email
        );

        return Result<CreateProfessionalResponse>.Success(response);
    }
}
