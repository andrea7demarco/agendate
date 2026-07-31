using Microsoft.AspNetCore.Identity;
using WebApi.Features.Identity.Domain;
using WebApi.Features.Identity.Shared.Auth;
using WebApi.Features.Identity.Shared.Authorization;
using WebApi.Features.Identity.UseCases.Login;
using WebApi.Features.Patients.UseCases.CreatePatient;
using WebApi.Features.Professionals.UseCases.CreateProfessional;
using WebApi.Shared.Results;

namespace WebApi.Features.Identity.UseCases.CompleteRegistration;

public sealed class CompleteRegistrationHandler(
    UserManager<ApplicationUser> userManager,
    ITokenProvider tokenProvider,
    CreatePatientHandler createPatientHandler,
    CreateProfessionalHandler createProfessionalHandler
)
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<LoginResponse>> HandleAsync(
        string userId,
        CompleteRegistrationRequest request,
        CancellationToken ct
    )
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result<LoginResponse>.Failure(
                Error.NotFound("user.not_found", "No se encontro el usuario.")
            );

        var roles = await _userManager.GetRolesAsync(user);
        var currentRole = roles.FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(currentRole) && currentRole != request.RegistrationKind)
        {
            return Result<LoginResponse>.Failure(
                Error.Conflict(
                    "auth.role_already_selected",
                    "Este usuario ya tiene un tipo de cuenta asignado."
                )
            );
        }

        if (string.IsNullOrWhiteSpace(currentRole))
        {
            var addRoleResult = await _userManager.AddToRoleAsync(user, request.RegistrationKind);
            if (!addRoleResult.Succeeded)
            {
                return Result<LoginResponse>.Failure(
                    Error.Validation(
                        "auth.user_role_assignment_failed",
                        "No se pudo asignar el rol al usuario.",
                        addRoleResult
                            .Errors.GroupBy(x => x.Code)
                            .ToDictionary(g => g.Key, g => g.Select(x => x.Description).ToArray())
                    )
                );
            }
        }

        if (request.RegistrationKind == IdentityRoles.PACIENTE)
        {
            var patientHealthInsurances =
                request
                    .HealthInsurances?.Select(x => new PatientHealthInsuranceRequest(
                        x.HealthInsuranceId,
                        x.AffiliateNumber,
                        x.PlanName
                    ))
                    .ToList()
                ?? [];

            var patientResult = await createPatientHandler.HandleAsync(
                new CreatePatientRequest(
                    ApplicationUserId: user.Id,
                    BirthDate: request.BirthDate!.Value,
                    Gender: request.Gender!.Value,
                    Email: user.Email ?? string.Empty,
                    FirstName: user.FirstName ?? string.Empty,
                    LastName: user.LastName ?? string.Empty,
                    HasCud: request.HasCud ?? false,
                    HealthInsurances: patientHealthInsurances
                ),
                ct
            );

            if (patientResult.IsFailure)
                return Result<LoginResponse>.Failure(patientResult.Error!);

            user.RegistrationCompleted = true;
        }

        if (request.RegistrationKind == IdentityRoles.PROFESIONAL)
        {
            var locations =
                request
                    .Locations?.Select(x => new ProfessionalLocationRequest(
                        x.Name,
                        x.FormattedAddress,
                        x.Street,
                        x.StreetNumber,
                        x.City,
                        x.Province,
                        x.PostalCode,
                        x.Latitude,
                        x.Longitude,
                        x.ExternalPlaceId,
                        x.ExternalProvider,
                        x.Instructions
                    ))
                    .ToList()
                ?? [];

            var availabilities =
                request
                    .Availabilities?.Select(x => new ProfessionalAvailabilityRequest(
                        x.DayOfWeek,
                        x.TimeSlot
                    ))
                    .ToList()
                ?? [];

            var trainings =
                request
                    .Trainings?.Select(x => new ProfessionalTrainingRequest(
                        x.Title,
                        x.Institution,
                        x.Year,
                        x.Description
                    ))
                    .ToList()
                ?? [];

            var professionalResult = await createProfessionalHandler.HandleAsync(
                new CreateProfessionalRequest(
                    ApplicationUserId: user.Id,
                    FirstName: user.FirstName ?? string.Empty,
                    LastName: user.LastName ?? string.Empty,
                    Email: user.Email ?? string.Empty,
                    Dni: request.Dni!,
                    PhoneNumber: request.PhoneNumber!,
                    ConsultationCost: request.ConsultationCost!.Value,
                    AppointmentType: request.AppointmentType!,
                    Address: request.Address,
                    Province: request.Province!,
                    City: request.City!,
                    NationalLicense: request.NationalLicense!,
                    ProvincialLicense: request.ProvincialLicense!,
                    Biography: request.Biography,
                    DegreeTitle: request.DegreeTitle,
                    University: request.University,
                    GraduationYear: request.GraduationYear,
                    SpecialtyIds: request.SpecialtyIds!,
                    HealthInsuranceIds: request.HealthInsuranceIds,
                    Locations: locations,
                    Availabilities: availabilities,
                    PatientGroups: request.PatientGroups,
                    Trainings: trainings
                ),
                ct
            );

            if (professionalResult.IsFailure)
                return Result<LoginResponse>.Failure(professionalResult.Error!);

            user.RegistrationCompleted = true;
        }

        var role = request.RegistrationKind;
        var accessToken = tokenProvider.CreateAccessToken(user, role);

        return Result<LoginResponse>.Success(
            new LoginResponse(
                AccessToken: accessToken,
                User: new AuthenticatedUserDto(
                    UserId: user.Id,
                    Email: user.Email ?? string.Empty,
                    FirstName: user.FirstName,
                    LastName: user.LastName,
                    Role: role,
                    RegistrationCompleted: user.RegistrationCompleted
                )
            )
        );
    }
}
