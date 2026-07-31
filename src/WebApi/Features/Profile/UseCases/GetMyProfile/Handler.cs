using Microsoft.EntityFrameworkCore;
using WebApi.Features.Professionals.Domain;
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
                patient.HasCud,
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
            .Include(p => p.Availabilities)
            .Include(p => p.PatientGroups)
            .Include(p => p.Trainings)
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
                professional.DegreeTitle,
                professional.University,
                professional.GraduationYear,
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
                Availabilities = professional.Availabilities.Select(availability => new
                {
                    DayOfWeek = (int)availability.DayOfWeek,
                    DayName = GetDayName(availability.DayOfWeek),
                    TimeSlot = (int)availability.TimeSlot,
                    TimeSlotName = GetTimeSlotName(availability.TimeSlot),
                }),
                PatientGroups = professional.PatientGroups.Select(patientGroup => new
                {
                    Id = (int)patientGroup.PatientGroup,
                    Name = GetPatientGroupName(patientGroup.PatientGroup),
                }),
                Trainings = professional
                    .Trainings.Where(training => training.IsActive)
                    .Select(training => new
                    {
                        training.Id,
                        training.Title,
                        training.Institution,
                        training.Year,
                        training.Description,
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

    private static string GetDayName(DayOfWeek dayOfWeek) =>
        dayOfWeek switch
        {
            DayOfWeek.Monday => "Lunes",
            DayOfWeek.Tuesday => "Martes",
            DayOfWeek.Wednesday => "Miercoles",
            DayOfWeek.Thursday => "Jueves",
            DayOfWeek.Friday => "Viernes",
            DayOfWeek.Saturday => "Sabado",
            DayOfWeek.Sunday => "Domingo",
            _ => dayOfWeek.ToString(),
        };

    private static string GetTimeSlotName(ProfessionalTimeSlot timeSlot) =>
        timeSlot switch
        {
            ProfessionalTimeSlot.Morning => "Maniana",
            ProfessionalTimeSlot.Midday => "Mediodia",
            ProfessionalTimeSlot.Afternoon => "Tarde",
            ProfessionalTimeSlot.Night => "Noche",
            _ => timeSlot.ToString(),
        };

    private static string GetPatientGroupName(PatientGroup patientGroup) =>
        patientGroup switch
        {
            PatientGroup.Babies => "Bebes",
            PatientGroup.Children => "Ninios",
            PatientGroup.Teenagers => "Adolescentes",
            PatientGroup.Adults => "Adultos",
            PatientGroup.OlderAdults => "Personas mayores",
            _ => patientGroup.ToString(),
        };
}
