using Microsoft.EntityFrameworkCore;
using WebApi.Features.Professionals.Domain;
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
                p.DegreeTitle,
                p.University,
                p.GraduationYear,
                p.ProfessionalSpecialties.Select(ps => new SpecialtyResponse(
                        ps.Specialty.Id,
                        ps.Specialty.Name,
                        ps.Specialty.ParentSpecialtyId
                    ))
                    .ToList(),
                p.Locations.Where(location => location.IsActive)
                    .Select(location => new ProfessionalLocationResponse(
                        location.Id,
                        location.Name,
                        location.FormattedAddress,
                        location.City,
                        location.Province,
                        location.Latitude,
                        location.Longitude,
                        location.ExternalPlaceId,
                        location.ExternalProvider,
                        location.Instructions
                    ))
                    .ToList(),
                p.Availabilities.Select(availability => new ProfessionalAvailabilityResponse(
                        (int)availability.DayOfWeek,
                        GetDayName(availability.DayOfWeek),
                        (int)availability.TimeSlot,
                        GetTimeSlotName(availability.TimeSlot)
                    ))
                    .ToList(),
                p.PatientGroups.Select(patientGroup => new ProfessionalPatientGroupResponse(
                        (int)patientGroup.PatientGroup,
                        GetPatientGroupName(patientGroup.PatientGroup)
                    ))
                    .ToList(),
                p.Trainings.Where(training => training.IsActive)
                    .Select(training => new ProfessionalTrainingResponse(
                        training.Id,
                        training.Title,
                        training.Institution,
                        training.Year,
                        training.Description
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

    private static string GetDayName(DayOfWeek dayOfWeek) =>
        dayOfWeek switch
        {
            DayOfWeek.Monday => "Lunes",
            DayOfWeek.Tuesday => "Martes",
            DayOfWeek.Wednesday => "Miércoles",
            DayOfWeek.Thursday => "Jueves",
            DayOfWeek.Friday => "Viernes",
            DayOfWeek.Saturday => "Sábado",
            DayOfWeek.Sunday => "Domingo",
            _ => dayOfWeek.ToString(),
        };

    private static string GetTimeSlotName(ProfessionalTimeSlot timeSlot) =>
        timeSlot switch
        {
            ProfessionalTimeSlot.Morning => "Mañana",
            ProfessionalTimeSlot.Midday => "Mediodía",
            ProfessionalTimeSlot.Afternoon => "Tarde",
            ProfessionalTimeSlot.Night => "Noche",
            _ => timeSlot.ToString(),
        };

    private static string GetPatientGroupName(PatientGroup patientGroup) =>
        patientGroup switch
        {
            PatientGroup.Babies => "Bebés",
            PatientGroup.Children => "Niños",
            PatientGroup.Teenagers => "Adolescentes",
            PatientGroup.Adults => "Adultos",
            PatientGroup.OlderAdults => "Personas mayores",
            _ => patientGroup.ToString(),
        };
}
